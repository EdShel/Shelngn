using Shelngn.Entities;
using Shelngn.Exceptions;
using Shelngn.Services.GameProjects;
using Shelngn.Services.GameProjects.Files;

namespace Shelngn.Services.Workspaces.ProjectFiles
{
    public class WorkspaceDirectory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<WorkspaceDirectory> Directories { get; set; }
        public IList<WorkspaceFile> Files { get; set; }
    }

    public class WorkspaceFile
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ProjectFilesWorkspaceState
    {
        public Uri ProjectFilesRoot { get; set; }
        public WorkspaceDirectory Root { get; set; }
    }

    public class ProjectFilesWorkspaceStateReducer
    {
        private readonly IGameProjectSearcher gameProjectSearcher;
        private readonly IFileSystem fileSystem;

        public ProjectFilesWorkspaceStateReducer(IGameProjectSearcher gameProjectSearcher, IFileSystem fileSystem)
        {
            this.gameProjectSearcher = gameProjectSearcher;
            this.fileSystem = fileSystem;
        }

        public async Task<ProjectFilesWorkspaceState> GetInitialState(Guid workspaceId)
        {
            Uri workspaceFolder = await GetWorkspaceRootFolderAsync(workspaceId);
            ProjectDirectory rootDirectory = await this.fileSystem.ListDirectoryFilesAsync(workspaceFolder)
                ?? throw new NotFoundException("Project directory");

            return new ProjectFilesWorkspaceState
            {
                ProjectFilesRoot = workspaceFolder,
                Root = MapDirectory(rootDirectory, workspaceFolder.LocalPath)
            };
        }

        private async Task<Uri> GetWorkspaceRootFolderAsync(Guid workspaceId)
        {
            GameProject? gameProject = await this.gameProjectSearcher.GetByIdAsync(workspaceId)
                ?? throw new NotFoundException("Game project");
            Uri workspaceFolder = new Uri(gameProject.FilesLocation);
            return workspaceFolder;
        }

        private WorkspaceDirectory MapDirectory(ProjectDirectory directory, string absolutePath)
        {
            return new WorkspaceDirectory
            {
                Id = GetResourceId(directory.FullPath, absolutePath),
                Name = Path.GetFileName(directory.FullPath),
                Directories = directory.Directories.Select(dir => MapDirectory(dir, absolutePath)).ToList(),
                Files = directory.Files.Select(file => MapFile(file, absolutePath)).ToList()
            };
        }

        private WorkspaceFile MapFile(ProjectFile file, string absolutePath)
        {
            return new WorkspaceFile
            {
                Id = GetResourceId(file.FullPath, absolutePath),
                Name = Path.GetFileName(file.FullPath),
            };
        }

        private static string GetResourceId(string fullPath, string workspaceFolder)
        {
            return Path.GetRelativePath(workspaceFolder, fullPath).Replace('\\', '/');
        }

        private static string GetResourcePath(string id, string workspaceFolder)
        {
            return Path.Combine(workspaceFolder, id);
        }

        public async Task FileUploaded(ProjectFilesWorkspaceState state, Guid workspaceId)
        {
            Uri workspaceFolder = await GetWorkspaceRootFolderAsync(workspaceId);
            ProjectDirectory rootDirectory = await this.fileSystem.ListDirectoryFilesAsync(workspaceFolder)
                ?? throw new NotFoundException("Project directory");

            state.Root = MapDirectory(rootDirectory, workspaceFolder.LocalPath);
        }

        public async Task DeleteFileAsync(ProjectFilesWorkspaceState state, string fileId)
        {
            // TODO: add write lock
            // TODO: add rollback
            string[] fileAddress = fileId.Split('/');
            WorkspaceDirectory currentDir = state.Root;
            for(int i = 0; i < fileAddress.Length - 1; i++)
            {
                string nextDirectory = fileAddress[i];
                currentDir = currentDir.Directories.FirstOrDefault(d => d.Name == nextDirectory)
                    ?? throw new InvalidOperationException("Path doesn't exist");
            }
            WorkspaceFile deletedFile = currentDir.Files.FirstOrDefault(f => f.Name == fileAddress[^1])
                ?? throw new InvalidOperationException("No such file in directory");
            currentDir.Files.Remove(deletedFile);

            string fileFullPath = GetResourcePath(fileId, state.ProjectFilesRoot.LocalPath);
            await fileSystem.DeleteFileIfExistsAsync(new Uri(fileFullPath));
        }
    }
}
