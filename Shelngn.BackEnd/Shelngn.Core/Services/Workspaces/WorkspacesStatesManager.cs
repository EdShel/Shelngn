using Microsoft.Extensions.Logging;
using Shelngn.Services.Workspaces.ActiveUsers;
using Shelngn.Services.Workspaces.ProjectFiles;
using System.Collections.Concurrent;

namespace Shelngn.Services.Workspaces
{
    public class WorkspacesStatesManager
    {
        private readonly ConcurrentDictionary<string, WorkspaceState> workspaces = new();

        private readonly ActiveUsersWorkspaceStateReducer activeUsersWorkspaceStateReducer;
        private readonly ProjectFilesWorkspaceStateReducer projectFilesWorkspaceStateReducer;

        private readonly ILogger<WorkspacesStatesManager> logger;

        public WorkspacesStatesManager(
            ActiveUsersWorkspaceStateReducer activeUsersWorkspaceStateReducer,
            ILogger<WorkspacesStatesManager> logger,
            ProjectFilesWorkspaceStateReducer projectFilesWorkspaceStateReducer)
        {
            this.activeUsersWorkspaceStateReducer = activeUsersWorkspaceStateReducer;
            this.logger = logger;
            this.projectFilesWorkspaceStateReducer = projectFilesWorkspaceStateReducer;
        }

        public async Task<WorkspaceState> AquireWorkspaceStateReferenceAsync(string workspaceId, string connectionId, Guid userId)
        {
            var workspaceState = this.workspaces.GetOrAdd(
                workspaceId,
                wId =>
                {
                    this.logger.LogInformation("Workspace state {WorkspaceId} is created", wId);
                    return InitializeWorkspaceState(wId).Result;
                }
            );
            await this.activeUsersWorkspaceStateReducer.AddUserAsync(connectionId, userId, workspaceState);
            return workspaceState;
        }

        private async Task<WorkspaceState> InitializeWorkspaceState(string workspaceId)
        {
            Guid workspaceIdGuid = Guids.FromUrlSafeBase64(workspaceId);
            WorkspaceState state = new WorkspaceState(
               activeUsers: await this.activeUsersWorkspaceStateReducer.GetInitialState(),
               projectFiles: await this.projectFilesWorkspaceStateReducer.GetInitialStateAsync(workspaceIdGuid)
            );
            return state;
        }

        public Task<WorkspaceState> GetWorkspaceAsync(string workspaceId, string connectionId)
        {
            if (!this.workspaces.TryGetValue(workspaceId, out WorkspaceState? workspaceState))
            {
                throw new InvalidOperationException("Workspace state does not exist. Are you sure it was aquired and not released?");
            }
            if (!workspaceState.ActiveUsers.IsContainingConnection(connectionId))
            {
                throw new InvalidOperationException("The workspace state isn't aquired by the connection.");
            }
            return Task.FromResult(workspaceState);
        }

        public async Task<WorkspaceState> ReleaseWorkspaceStateReferenceAsync(string workspaceId, string connectionId)
        {
            if (!this.workspaces.TryGetValue(workspaceId, out WorkspaceState? workspaceState))
            {
                throw new InvalidOperationException("Workspace state isn't initialized.");
            }

            await this.activeUsersWorkspaceStateReducer.RemoveUserAsync(connectionId, workspaceState);
            Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ =>
            {
                if (!workspaceState.ActiveUsers.IsAnyUserConnected())
                {
                    this.logger.LogInformation("Workspace state {WorkspaceId} is destroyed", workspaceId);
                    this.workspaces.Remove(workspaceId, out var _);
                }
            }).GetAwaiter();
            return workspaceState;
        }
    }
}
