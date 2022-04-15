using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shelngn.Api.GameProjects;
using Shelngn.Entities;
using Shelngn.Exceptions;
using Shelngn.Services.GameProjects;
using Shelngn.Services.GameProjects.Authorization;
using Shelngn.Services.GameProjects.Build;
using Shelngn.Services.GameProjects.Files;
using Shelngn.Services.Workspaces;
using Shelngn.Services.Workspaces.ProjectFiles;
using System.Text;

namespace Shelngn.Api.Workspaces
{
    public partial class WorkspaceHub
    {
        [HubMethodName("build")]
        public async Task BuildProject()
        {
            var builder = Resolve<IGameProjectBuilder>();

            await DispatchToWorkspaceAsync(new
            {
                type = "wokspace/build/begin"
            });

            string workspaceId = this.WorkspaceId;
            BuildResult result = await builder.BuildProjectAsync(workspaceId);

            if (result.IsSuccess)
            {
                await DispatchToWorkspaceAsync(new
                {
                    type = "wokspace/build/finish"
                });
            }
            else
            {
                await DispatchToWorkspaceAsync(new
                {
                    type = "wokspace/build/failed",
                    error = result.Error
                });
            }
        }
    }

    public partial class WorkspaceHub
    {

        [HubMethodName("ls")]
        public async Task ListAllFiles()
        {
            WorkspaceState state = await GetWorkspaceStateAsync();

            await DispatchToCallerAsync(new
            {
                type = "workspace/ls",
                projectFiles = state.ProjectFiles.Root
            });
        }

        [HubMethodName("deleteFile")]
        public async Task DeleteFile(string fileId)
        {
            WorkspaceState state = await GetWorkspaceStateAsync();
            var reducer = Resolve<ProjectFilesWorkspaceStateReducer>();

            await reducer.DeleteFileAsync(state.ProjectFiles, fileId);

            await DispatchToWorkspaceAsync(new
            {
                type = "workspace/ls",
                projectFiles = state.ProjectFiles.Root
            });
        }

        [HubMethodName("uploadFile")]
        public async Task UploadFile()
        {
            WorkspaceState state = await GetWorkspaceStateAsync();
            var reducer = Resolve<ProjectFilesWorkspaceStateReducer>();

            await reducer.FileUploaded(state.ProjectFiles, this.WorkspaceIdGuid);

            await DispatchToWorkspaceAsync(new
            {
                type = "workspace/ls",
                projectFiles = state.ProjectFiles.Root
            });
        }

        [HubMethodName("moveFile")]
        public async Task MoveFile(string fileId, string folderId)
        {
            WorkspaceState state = await GetWorkspaceStateAsync();
            var reducer = Resolve<ProjectFilesWorkspaceStateReducer>();

            await reducer.MoveFileAsync(state.ProjectFiles, fileId, folderId);

            await DispatchToWorkspaceAsync(new
            {
                type = "workspace/ls",
                projectFiles = state.ProjectFiles.Root
            });
        }

        [HubMethodName("createFolder")]
        public async Task CreateFolder(string containingFolderId, string folderName)
        {
            WorkspaceState state = await GetWorkspaceStateAsync();
            var reducer = Resolve<ProjectFilesWorkspaceStateReducer>();

            await reducer.CreateFolderAsync(state.ProjectFiles, containingFolderId, folderName);

            await DispatchToWorkspaceAsync(new
            {
                type = "workspace/ls",
                projectFiles = state.ProjectFiles.Root
            });
        }

        [HubMethodName("deleteFolder")]
        public async Task DeleteFolder(string folderId)
        {
            WorkspaceState state = await GetWorkspaceStateAsync();
            var reducer = Resolve<ProjectFilesWorkspaceStateReducer>();

            await reducer.DeleteFolderAsync(state.ProjectFiles, folderId);

            await DispatchToWorkspaceAsync(new
            {
                type = "workspace/ls",
                projectFiles = state.ProjectFiles.Root
            });
        }

        [HubMethodName("moveFolder")]
        public async Task MoveFolder(string movedFolderId, string newContainingFolderId)
        {
            WorkspaceState state = await GetWorkspaceStateAsync();
            var reducer = Resolve<ProjectFilesWorkspaceStateReducer>();

            await reducer.MoveFolderAsync(state.ProjectFiles, movedFolderId, newContainingFolderId);

            await DispatchToWorkspaceAsync(new
            {
                type = "workspace/ls",
                projectFiles = state.ProjectFiles.Root
            });
        }

        [HubMethodName("createFile")]
        public async Task CreateFile(string folderId, string fileName)
        {
            WorkspaceState state = await GetWorkspaceStateAsync();
            var reducer = Resolve<ProjectFilesWorkspaceStateReducer>();

            await reducer.CreateEmptyFileAsync(state.ProjectFiles, folderId, fileName);

            await DispatchToWorkspaceAsync(new
            {
                type = "workspace/ls",
                projectFiles = state.ProjectFiles.Root
            });
        }

        [HubMethodName("dumpFile")]
        public async Task DumpFileContent(string fileId, string content)
        {
            IFileSystem fileSystem = Resolve<IFileSystem>();
            WorkspaceState state = await GetWorkspaceStateAsync();
            var reducer = Resolve<ProjectFilesWorkspaceStateReducer>();
            string path = reducer.GetResourcePath(fileId, state.ProjectFiles.ProjectFilesRoot);

            await fileSystem.CreateOrOverwriteFileAsync(path, Encoding.UTF8.GetBytes(content));
        }
    }

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
