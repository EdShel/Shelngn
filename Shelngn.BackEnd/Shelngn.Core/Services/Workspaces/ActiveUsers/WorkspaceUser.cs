namespace Shelngn.Services.Workspaces.ActiveUsers
{
    public record WorkspaceUser(
        string ConnectionId,
        Guid UserId,
        string UserName
    );
}
