using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shelngn.Api.GameProjects;
using Shelngn.Entities;
using Shelngn.Exceptions;
using Shelngn.Services.GameProjects;
using Shelngn.Services.Workspaces;

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

        private Guid WorkspaceIdGuid => GuidExtensions.FromUrlSafeBase64(this.WorkspaceId);

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
            // TODO: validate access to game project
            string workspaceId = this.WorkspaceId;
            string userId = this.Context.User.GetIdString();
            Guid userIdGuid = this.Context.User.GetIdGuid();
            string connectionId = this.Context.ConnectionId;

            this.logger.LogInformation("User {UserId} connected {WorkspaceId} with connection id {ConnectionId}.", userId, workspaceId, connectionId);

            WorkspaceState workspace = null;
            //try
            //{
            workspace = await this.workspacesStatesManager.AquireWorkspaceStateReferenceAsync(workspaceId, connectionId, userIdGuid);
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

            Guid gameProjectId = GuidExtensions.FromUrlSafeBase64(this.WorkspaceId);
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
            IGameProjectSearcher gameProjectSearcher = Resolve<IGameProjectSearcher>();
            IFileSystem fileSystem = Resolve<IFileSystem>();

            Guid gameProjectId = GuidExtensions.FromUrlSafeBase64(this.WorkspaceId);
            GameProject? gameProject = await gameProjectSearcher.GetByIdAsync(gameProjectId)
                ?? throw new NotFoundException("Game project");
            ProjectDirectory files = await fileSystem.ListDirectoryFilesAsync(new Uri(gameProject.FilesLocation))
                ?? throw new NotFoundException("Project directory");

            await this.Clients.Caller.SendAsync("redux", new
            {
                type = "workspace/ls",
                projectFiles = files
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
    }
}
