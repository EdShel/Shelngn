namespace Shelngn.Services.Workspaces
{
    public record WorkspaceUser(
        string ConnectionId,
        Guid UserId,
        string UserName
    );
}
