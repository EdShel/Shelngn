using Microsoft.Extensions.Logging;
using Shelngn.Services.Workspaces.ActiveUsers;
using System.Collections.Concurrent;

namespace Shelngn.Services.Workspaces
{
    public class WorkspacesStatesManager
    {
        private readonly ConcurrentDictionary<string, WorkspaceStateItem> workspaces = new();

        private readonly ActiveUsersWorkspaceStateReducer activeUsersWorkspaceStateReducer;
        private readonly ILogger<WorkspacesStatesManager> logger;

        public WorkspacesStatesManager(ActiveUsersWorkspaceStateReducer activeUsersWorkspaceStateReducer, ILogger<WorkspacesStatesManager> logger)
        {
            this.activeUsersWorkspaceStateReducer = activeUsersWorkspaceStateReducer;
            this.logger = logger;
        }

        public async Task<WorkspaceState> AquireWorkspaceStateReferenceAsync(string workspaceId, string connectionId, Guid userId)
        {
            var workspaceItem = this.workspaces.GetOrAdd(
                workspaceId,
                wId =>
                {
                    this.logger.LogInformation("Workspace state {WorkspaceId} is created", wId);
                    return new WorkspaceStateItem();
                }
            );
            await this.activeUsersWorkspaceStateReducer.AddUserAsync(connectionId, userId, workspaceItem.WorkspaceState);
            return workspaceItem.WorkspaceState;
        }

        public Task<WorkspaceState> GetWorkspaceAsync(string workspaceId, string connectionId)
        {
            if (!this.workspaces.TryGetValue(workspaceId, out WorkspaceStateItem? workspaceStateItem))
            {
                throw new InvalidOperationException("Workspace state does not exist. Are you sure it was aquired and not released?");
            }
            if (!workspaceStateItem.WorkspaceState.ActiveUsers.IsContainingConnection(connectionId))
            {
                throw new InvalidOperationException("The workspace state isn't aquired by the connection.");
            }
            return Task.FromResult(workspaceStateItem.WorkspaceState);
        }

        public async Task<WorkspaceState> ReleaseWorkspaceStateReferenceAsync(string workspaceId, string connectionId)
        {
            if (!this.workspaces.TryGetValue(workspaceId, out WorkspaceStateItem? workspaceItem))
            {
                throw new InvalidOperationException("Workspace state isn't initialized.");
            }

            await this.activeUsersWorkspaceStateReducer.RemoveUserAsync(connectionId, workspaceItem.WorkspaceState);
            Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ =>
            {
                if (!workspaceItem.WorkspaceState.ActiveUsers.IsAnyUserConnected())
                {
                    this.logger.LogInformation("Workspace state {WorkspaceId} is destroyed", workspaceId);
                    this.workspaces.Remove(workspaceId, out var _);
                }
            }).GetAwaiter();
            return workspaceItem.WorkspaceState;
        }

        private class WorkspaceStateItem
        {
            public readonly WorkspaceState WorkspaceState = new WorkspaceState();
        }
    }
}
