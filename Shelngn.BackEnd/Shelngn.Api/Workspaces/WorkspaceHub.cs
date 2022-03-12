using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shelngn.Services.Workspaces;

namespace Shelngn.Api.Workspaces
{
    [Authorize]
    public class WorkspaceHub : Hub
    {
        private readonly ILogger<WorkspaceHub> logger;
        private readonly WorkspacesStatesManager workspacesStatesManager;

        public WorkspaceHub(ILogger<WorkspaceHub> logger, WorkspacesStatesManager workspacesStatesManager)
        {
            this.logger = logger;
            this.workspacesStatesManager = workspacesStatesManager;
        }

        private string WorkspaceId => this.Context.GetHttpContext()?.Request.Query["w"]
            ?? throw new ArgumentNullException("Connected without specifying workspace id.");

        public override async Task OnConnectedAsync()
        {
            string workspaceId = this.WorkspaceId;
            string userId = this.Context.User.GetIdString();
            Guid userIdGuid = this.Context.User.GetIdGuid();
            string connectionId = this.Context.ConnectionId;

            this.logger.LogInformation("User {UserId} connected {WorkspaceId} with connection id {ConnectionId}.", userId, workspaceId, connectionId);

            WorkspaceState workspace = null;
            //try
            //{
            workspace = await workspacesStatesManager.AquireWorkspaceStateReferenceAsync(workspaceId, connectionId, userIdGuid);
            //}
            //catch (Exception)
            //{
            //    if (workspace != null)
            //    {
            //        await workspacesStatesManager.ReleaseWorkspaceStateReferenceAsync(workspaceId);
            //    }
            //    throw;
            //}

            await this.Groups.AddToGroupAsync(connectionId, workspaceId);

            await this.Clients.Group(workspaceId).SendAsync("redux", new
            {
                type = "workspace/userConnected",
                users = workspace.ActiveUsers.GetUsers()
            });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string workspaceId = this.WorkspaceId;
            string userId = this.Context.User.GetIdString();
            Guid userIdGuid = this.Context.User.GetIdGuid();
            string connectionId = this.Context.ConnectionId;

            this.logger.LogInformation(exception, "User {UserId} disconnected {WorkspaceId} with connection id {ConnectionId}.", userId, workspaceId, connectionId);

            WorkspaceState workspace = await this.workspacesStatesManager.ReleaseWorkspaceStateReferenceAsync(workspaceId, connectionId);

            await this.Clients.Group(workspaceId).SendAsync("redux", new
            {
                type = "workspace/userDisconnected",
                users = workspace.ActiveUsers.GetUsers()
            });
        }
    }
}
