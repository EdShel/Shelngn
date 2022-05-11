using Microsoft.AspNetCore.SignalR;
using Shelngn.Services.GameProjects.Files;
using Shelngn.Services.Workspaces;
using Shelngn.Services.Workspaces.ProjectFiles;
using System.Text;

namespace Shelngn.Api.Workspaces
{
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

            await reducer.FileUploadedAsync(state.ProjectFiles, this.WorkspaceIdGuid);

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

            await DispatchToOthersInWorkspaceAsync(new
            {
                type = "workspace/readFile",
                fileId = fileId,
                content = content
            });
        }

        [HubMethodName("readFile")]
        public async Task ReadFileContent(string fileId)
        {
            IFileSystem fileSystem = Resolve<IFileSystem>();
            WorkspaceState state = await GetWorkspaceStateAsync();
            var reducer = Resolve<ProjectFilesWorkspaceStateReducer>();
            string path = reducer.GetResourcePath(fileId, state.ProjectFiles.ProjectFilesRoot);

            byte[] fileContent = await fileSystem.ReadFileAsync(path);
            string textContent = Encoding.UTF8.GetString(fileContent);

            await DispatchToCallerAsync(new
            {
                type = "workspace/readFile",
                fileId = fileId,
                content = textContent
            });
        }
    }
}
