namespace Shelngn.Services.Workspaces
{
    public interface IWorkspacesStatesManager
    {
        Task<WorkspaceState> AquireWorkspaceStateReferenceAsync(string workspaceId, string connectionId, Guid userId);
        Task<WorkspaceState> ReleaseWorkspaceStateReferenceAsync(string workspaceId, string connectionId);
        Task<WorkspaceState> GetWorkspaceAsync(string workspaceId, string connectionId);
    }
}
