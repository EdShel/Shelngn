using Shelngn.Services.Workspaces;
using Shelngn.Services.Workspaces.ActiveUsers;

namespace Shelngn.Business.Workspaces.ActiveUsers
{
    public class ActiveUsersWorkspaceState : IActiveUsersWorkspaceState
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
