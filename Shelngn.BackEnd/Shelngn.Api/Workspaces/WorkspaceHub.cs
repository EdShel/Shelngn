using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shelngn.Api.GameProjects;
using Shelngn.Entities;
using Shelngn.Exceptions;
using Shelngn.Services.GameProjects.Authorization;
using Shelngn.Services.GameProjects.Crud;
using Shelngn.Services.Workspaces;

namespace Shelngn.Api.Workspaces
{
    [Authorize]
    public partial class WorkspaceHub : Hub
    {
        private readonly ILogger<WorkspaceHub> logger;
        private readonly WorkspacesStatesManager workspacesStatesManager;
        private readonly IServiceProvider serviceProvider;

        public WorkspaceHub(ILogger<WorkspaceHub> logger, WorkspacesStatesManager workspacesStatesManager, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.workspacesStatesManager = workspacesStatesManager;
            this.serviceProvider = serviceProvider;
        }

        private string WorkspaceId => this.Context.GetHttpContext()?.Request.Query["w"]
            ?? throw new ArgumentNullException("Connected without specifying workspace id.");

        private Guid WorkspaceIdGuid => Guids.FromUrlSafeBase64(this.WorkspaceId);

        private T Resolve<T>() where T : class
        {
            return this.serviceProvider.GetRequiredService<T>();
        }

        private async Task<WorkspaceState> GetWorkspaceStateAsync()
        {
            return await this.workspacesStatesManager.GetWorkspaceAsync(this.WorkspaceId, this.Context.ConnectionId);
        }

        private async Task DispatchToWorkspaceAsync(object action)
        {
            await this.Clients.Group(this.WorkspaceId).SendAsync("redux", action);
        }

        private async Task DispatchToCallerAsync(object action)
        {
            await this.Clients.Caller.SendAsync("redux", action);
        }

        private async Task DispatchToOthersInWorkspaceAsync(object action)
        {
            await this.Clients.OthersInGroup(this.WorkspaceId).SendAsync("redux", action);
        }

        public override async Task OnConnectedAsync()
        {
            string workspaceId = this.WorkspaceId;
            string userId = this.Context.User.GetIdString();
            Guid userIdGuid = this.Context.User.GetIdGuid();
            string connectionId = this.Context.ConnectionId;

            IGameProjectAuthorizer gameProjectAuthorizer = Resolve<IGameProjectAuthorizer>();
            GameProjectRights rights = await gameProjectAuthorizer.GetRightsForUserAsync(userIdGuid, this.WorkspaceIdGuid);
            if (!rights.Workspace)
            {
                this.logger.LogInformation(
                    "Forbidden access to workspace for user {UserId} {WorkspaceId} with connection id {ConnectionId}.",
                    userId, workspaceId, connectionId);
                this.Context.Abort();
                return;
            }

            this.logger.LogInformation("User {UserId} connected {WorkspaceId} with connection id {ConnectionId}.", userId, workspaceId, connectionId);
            WorkspaceState workspace = await this.workspacesStatesManager.AquireWorkspaceStateReferenceAsync(workspaceId, connectionId, userIdGuid);

            await this.Groups.AddToGroupAsync(connectionId, workspaceId);

            await DispatchToWorkspaceAsync(new
            {
                type = "workspace/userConnected",
                users = workspace.ActiveUsers.GetUsers()
            });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string workspaceId = this.WorkspaceId;
            string userId = this.Context.User.GetIdString();
            string connectionId = this.Context.ConnectionId;

            this.logger.LogInformation(exception, "User {UserId} disconnected {WorkspaceId} with connection id {ConnectionId}.", userId, workspaceId, connectionId);

            WorkspaceState workspace = await this.workspacesStatesManager.ReleaseWorkspaceStateReferenceAsync(workspaceId, connectionId);

            await DispatchToWorkspaceAsync(new
            {
                type = "workspace/userDisconnected",
                users = workspace.ActiveUsers.GetUsers()
            });
        }

        [HubMethodName("gameProject")]
        public async Task GetGameProject()
        {
            IGameProjectSearcher gameProjectSearcher = Resolve<IGameProjectSearcher>();
            IMapper mapper = Resolve<IMapper>();

            Guid gameProjectId = Guids.FromUrlSafeBase64(this.WorkspaceId);
            GameProject gameProject = await gameProjectSearcher.GetByIdAsync(gameProjectId)
                ?? throw new NotFoundException("Game project");
            GameProjectListModel gameProjectModel = mapper.Map<GameProjectListModel>(gameProject);

            await this.Clients.Caller.SendAsync("redux", new
            {
                type = "workspace/gameProject",
                gameProject = gameProjectModel
            });
        }

        [HubMethodName("renameProject")]
        public async Task RenameGameProject(string newProjectName)
        {
            IGameProjectUpdater gameProjectUpdater = Resolve<IGameProjectUpdater>();

            await gameProjectUpdater.UpdateNameAsync(this.WorkspaceIdGuid, newProjectName);

            await DispatchToWorkspaceAsync(new
            {
                type = "workspace/gameProject/rename",
                newProjectName
            });
        }
    }
}
