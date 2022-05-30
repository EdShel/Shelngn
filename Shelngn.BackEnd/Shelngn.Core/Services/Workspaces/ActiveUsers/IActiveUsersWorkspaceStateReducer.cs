namespace Shelngn.Services.Workspaces.ActiveUsers
{
    public interface IActiveUsersWorkspaceStateReducer
    {
        Task<IActiveUsersWorkspaceState> GetInitialState();
        Task AddUserAsync(IActiveUsersWorkspaceState workspaceState, string connectionId, Guid userId);
        Task RemoveUserAsync(IActiveUsersWorkspaceState workspaceState, string connectionId);
    }
}
