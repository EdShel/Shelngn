using Shelngn.Models;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Shelngn.Services.Workspaces
{
    public record WorkspaceUser(
        string ConnectionId,
        Guid UserId,
        string UserName
    );

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

    public class WorkspaceState
    {
        public ActiveUsersWorkspaceState ActiveUsers { get; private set; } = new();
    }

    public class ActiveUsersWorkspaceStateReducer
    {
        private readonly IAppUserStore appUserStore;

        public ActiveUsersWorkspaceStateReducer(IAppUserStore appUserStore)
        {
            this.appUserStore = appUserStore;
        }

        public async Task AddUserAsync(string connectionId, Guid userId, WorkspaceState workspaceState)
        {
            AppUser user = await this.appUserStore.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");
            WorkspaceUser workspaceUser = new(connectionId, userId, user.UserName);
            workspaceState.ActiveUsers.AddUser(workspaceUser);
        }

        public Task RemoveUserAsync(string connectionId, WorkspaceState workspaceState)
        {
            workspaceState.ActiveUsers.RemoveUser(connectionId);
            return Task.CompletedTask;
        }
    }

    public class ActiveUsersWorkspaceState
    {
        private readonly List<WorkspaceUser> connectedUsers = new List<WorkspaceUser>();
        private readonly ReaderWriterLockSlim rwLock = new();

        public void AddUser(WorkspaceUser user)
        {
            this.rwLock.EnterWriteLock();
            try
            {
                if (this.connectedUsers.Any(u => u.ConnectionId == user.ConnectionId))
                {
                    throw new InvalidOperationException("User with the connection id is already added.");
                }
                this.connectedUsers.Add(user);
            }
            finally
            {
                this.rwLock.ExitWriteLock();
            }
        }

        public void RemoveUser(string connectionId)
        {
            this.rwLock.EnterWriteLock();
            try
            {
                if (!this.connectedUsers.Any(u => u.ConnectionId == connectionId))
                {
                    throw new InvalidOperationException("The user with the connection is not connected.");
                }
                this.connectedUsers.RemoveAll(u => u.ConnectionId == connectionId);
            }
            finally
            {
                this.rwLock.ExitWriteLock();
            }
        }

        public bool IsContainingConnection(string connectionId)
        {
            // TODO: maybe need to use concurrent dictionary to speed up things
            this.rwLock.EnterReadLock();
            try
            {
                return this.connectedUsers.Any(u => u.ConnectionId == connectionId);
            }
            finally
            {
                this.rwLock.ExitReadLock();
            }
        }

        public bool IsAnyUserConnected()
        {
            this.rwLock.EnterReadLock();
            try
            {
                return this.connectedUsers.Count > 0;
            }
            finally
            {
                this.rwLock.ExitReadLock();
            }
        }

        public IEnumerable<WorkspaceUser> GetUsers()
        {
            this.rwLock.EnterReadLock();
            try
            {
                return this.connectedUsers.ToArray();
            }
            finally
            {
                this.rwLock.ExitReadLock();
            }
        }
    }
}
