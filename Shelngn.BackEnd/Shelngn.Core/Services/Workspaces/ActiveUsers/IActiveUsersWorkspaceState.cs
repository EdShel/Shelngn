namespace Shelngn.Services.Workspaces.ActiveUsers
{
    public interface IActiveUsersWorkspaceState
    {
        IEnumerable<WorkspaceUser> GetUsers();
        void AddUser(WorkspaceUser user);
        bool IsAnyUserConnected();
        bool IsContainingConnection(string connectionId);
        void RemoveUser(string connectionId);
    }
}
