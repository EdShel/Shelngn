using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Shelngn.Api.Workspaces
{
    [Authorize]
    public class WorkspaceHub : Hub
    {
        private readonly ILogger<WorkspaceHub> logger;

        public WorkspaceHub(ILogger<WorkspaceHub> logger)
        {
            this.logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            this.logger.LogInformation("Someone connected " + this.Context.ConnectionId);
            await this.Clients.All.SendAsync("ReceiveShit", new { Lol = "kek" });
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            this.logger.LogInformation("Disconnected shit");
            return Task.CompletedTask;
        }

    }
}
