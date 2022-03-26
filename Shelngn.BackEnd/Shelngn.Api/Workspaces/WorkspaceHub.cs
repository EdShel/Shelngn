using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shelngn.Api.GameProjects;
using Shelngn.Entities;
using Shelngn.Exceptions;
using Shelngn.Services.GameProjects;
using Shelngn.Services.GameProjects.Authorization;
using Shelngn.Services.Workspaces;
using Shelngn.Services.Workspaces.ProjectFiles;

namespace Shelngn.Api.Workspaces
{
    [Authorize]
    public class WorkspaceHub : Hub
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

        private async Task DispatchToWorkspace(object action)
        {
            await this.Clients.Group(this.WorkspaceId).SendAsync("redux", action);
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

            await DispatchToWorkspace(new
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

            await DispatchToWorkspace(new
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

        [HubMethodName("ls")]
        public async Task ListAllFiles()
        {
            WorkspaceState state = await this.workspacesStatesManager.GetWorkspaceAsync(this.WorkspaceId, this.Context.ConnectionId);

            await this.Clients.Caller.SendAsync("redux", new
            {
                type = "workspace/ls",
                projectFiles = state.ProjectFiles.Root
            });
        }

        [HubMethodName("renameProject")]
        public async Task RenameGameProject(string newProjectName)
        {
            IGameProjectUpdater gameProjectUpdater = Resolve<IGameProjectUpdater>();

            await gameProjectUpdater.UpdateNameAsync(this.WorkspaceIdGuid, newProjectName);

            await DispatchToWorkspace(new
            {
                type = "workspace/gameProject/rename",
                newProjectName
            });
        }

        [HubMethodName("deleteFile")]
        public async Task DeleteFile(string fileId)
        {
            var state = await this.workspacesStatesManager.GetWorkspaceAsync(this.WorkspaceId, this.Context.ConnectionId);
            var reducer = Resolve<ProjectFilesWorkspaceStateReducer>();

            await reducer.DeleteFileAsync(state.ProjectFiles, fileId);

            await DispatchToWorkspace(new
            {
                type = "workspace/ls",
                projectFiles = state.ProjectFiles.Root
            });
        }
    }
}
