using Shelngn.Entities;
using Shelngn.Exceptions;
using Shelngn.Services.GameProjects;
using Shelngn.Services.GameProjects.Files;

namespace Shelngn.Services.Workspaces.ProjectFiles
{
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
            string workspaceFolder = await GetWorkspaceRootFolderAsync(workspaceId);
            ProjectDirectory rootDirectory = await this.fileSystem.ListDirectoryFilesAsync(workspaceFolder)
                ?? throw new NotFoundException("Project directory");

            var rootDir = MapDirectory(rootDirectory, workspaceFolder);
            rootDir.Directories = rootDir.Directories.Where(f => !IsReservedId(f.Id)).ToList();
            rootDir.Files = rootDir.Files.Where(f => !IsReservedId(f.Id)).ToList();

            return new ProjectFilesWorkspaceState
            {
                ProjectFilesRoot = workspaceFolder,
                Root = rootDir
            };
        }

        private bool IsReservedId(string id)
        {
            return id == "dist"
                || id == "index.js"
                || id == "webpack.config.js"
                || id == "shelngn.js";
        }

        private async Task<string> GetWorkspaceRootFolderAsync(Guid workspaceId)
        {
            GameProject? gameProject = await this.gameProjectSearcher.GetByIdAsync(workspaceId)
                ?? throw new NotFoundException("Game project");
            return gameProject.FilesLocation;
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

        public string GetResourcePath(string id, string workspaceFolder)
        {
            return Path.Combine(workspaceFolder, id);
        }

        public async Task FileUploaded(ProjectFilesWorkspaceState state, Guid workspaceId)
        {
            string workspaceFolder = await GetWorkspaceRootFolderAsync(workspaceId);
            ProjectDirectory rootDirectory = await this.fileSystem.ListDirectoryFilesAsync(workspaceFolder)
                ?? throw new NotFoundException("Project directory");

            state.Root = MapDirectory(rootDirectory, workspaceFolder);
        }

        public Task CreateEmptyFileAsync(ProjectFilesWorkspaceState state, string folderId, string fileName)
        {
            var containingDirectory = FindDirectoryState(state, folderId)
                ?? throw new InvalidOperationException("Directory doesn't exist.");
            if (containingDirectory.Files.Any(f => f.Name == fileName))
            {
                throw new InvalidOperationException("File with such name already exists.");
            }
            WorkspaceFile newFile = new WorkspaceFile
            {
                Id = containingDirectory.Id == "." ? fileName : containingDirectory.Id + "/" + fileName,
                Name = fileName
            };
            containingDirectory.Files.Add(newFile);

            string filePath = GetResourcePath(newFile.Id, state.ProjectFilesRoot);
            fileSystem.CreateEmptyFileAsync(filePath);

            return Task.CompletedTask;
        }

        public async Task DeleteFileAsync(ProjectFilesWorkspaceState state, string fileId)
        {
            // TODO: add write lock
            // TODO: add rollback
            (WorkspaceDirectory containingDir, WorkspaceFile file) = FindFileAndItsDirectoryState(state, fileId);
            containingDir.Files.Remove(file);

            string fileFullPath = GetResourcePath(fileId, state.ProjectFilesRoot);

            await fileSystem.DeleteFileIfExistsAsync(fileFullPath);
        }

        private (WorkspaceDirectory, WorkspaceFile) FindFileAndItsDirectoryState(ProjectFilesWorkspaceState state, string fileId)
        {
            string[] fileAddress = fileId.Split('/');
            WorkspaceDirectory currentDir = state.Root;
            for (int i = 0; i < fileAddress.Length - 1; i++)
            {
                string nextDirectory = fileAddress[i];
                currentDir = currentDir.Directories.FirstOrDefault(d => d.Name == nextDirectory)
                    ?? throw new InvalidOperationException("Path doesn't exist");
            }
            WorkspaceFile file = currentDir.Files.FirstOrDefault(f => f.Name == fileAddress[^1])
                ?? throw new InvalidOperationException("No such file in directory");

            return (currentDir, file);
        }

        private WorkspaceDirectory FindDirectoryState(ProjectFilesWorkspaceState state, string folderId)
        {
            if (folderId == ".")
            {
                return state.Root;
            }
            string[] folderAddress = folderId.Split('/');
            WorkspaceDirectory currentDir = state.Root;
            for (int i = 0; i < folderAddress.Length; i++)
            {
                string nextDirectory = folderAddress[i];
                currentDir = currentDir.Directories.FirstOrDefault(d => d.Name == nextDirectory)
                    ?? throw new InvalidOperationException("Path doesn't exist");
            }
            return currentDir;
        }

        private WorkspaceDirectory FindContainingDirectoryState(ProjectFilesWorkspaceState state, string folderOrFileId)
        {
            if (folderOrFileId == ".")
            {
                return state.Root;
            }
            string[] itemAddress = folderOrFileId.Split('/');
            WorkspaceDirectory currentDir = state.Root;
            for (int i = 0; i < itemAddress.Length - 1; i++)
            {
                string nextDirectory = itemAddress[i];
                currentDir = currentDir.Directories.FirstOrDefault(d => d.Name == nextDirectory)
                    ?? throw new InvalidOperationException("Path doesn't exist");
            }
            return currentDir;
        }

        public async Task MoveFileAsync(ProjectFilesWorkspaceState state, string fileId, string folderId)
        {
            (WorkspaceDirectory sourceDir, WorkspaceFile sourceFile) = FindFileAndItsDirectoryState(state, fileId);
            if (sourceDir.Id == folderId)
            {
                return;
            }
            sourceDir.Files.Remove(sourceFile);

            WorkspaceDirectory destinationDir = FindDirectoryState(state, folderId);
            WorkspaceFile? existingDestinationFile = destinationDir.Files.FirstOrDefault(f => f.Name == sourceFile.Name);
            if (existingDestinationFile != null)
            {
                destinationDir.Files.Remove(existingDestinationFile);
            }

            WorkspaceFile destinationFile = new WorkspaceFile
            {
                Name = sourceFile.Name,
                Id = folderId == "." ? sourceFile.Name : folderId + "/" + sourceFile.Name
            };
            destinationDir.Files.Add(destinationFile);

            string sourceUri = GetResourcePath(sourceFile.Id, state.ProjectFilesRoot);
            string destinationUri = GetResourcePath(destinationFile.Id, state.ProjectFilesRoot);
            await fileSystem.MoveFileAsync(sourceUri, destinationUri);
        }

        public async Task CreateFolderAsync(ProjectFilesWorkspaceState state, string containingFolderId, string folderName)
        {
            WorkspaceDirectory containgFolder = FindDirectoryState(state, containingFolderId);
            bool alredyContainsSuchDir = containgFolder.Directories.Any(d =>
                d.Name.Equals(folderName, StringComparison.OrdinalIgnoreCase));
            if (alredyContainsSuchDir)
            {
                return;
            }

            WorkspaceDirectory newSubDirectory = new WorkspaceDirectory
            {
                Id = containingFolderId == "." ? folderName : containingFolderId + "/" + folderName,
                Name = folderName,
                Files = new List<WorkspaceFile>(),
                Directories = new List<WorkspaceDirectory>(),
            };
            containgFolder.Directories.Add(newSubDirectory);

            string directoryPath = GetResourcePath(newSubDirectory.Id, state.ProjectFilesRoot);
            await fileSystem.CreateDirectoryOrDoNothingIfExistsAsync(directoryPath);
        }

        public async Task DeleteFolderAsync(ProjectFilesWorkspaceState state, string folderId)
        {
            WorkspaceDirectory containingFolder = FindContainingDirectoryState(state, folderId);
            if (containingFolder == state.Root)
            {
                throw new InvalidOperationException("Cannot delete the folder.");
            }

            WorkspaceDirectory folderToDelete = containingFolder.Directories.FirstOrDefault(d => d.Id == folderId)
                ?? throw new InvalidOperationException("The folder doesn't exist.");

            containingFolder.Directories.Remove(folderToDelete);

            string deletedDirectoryPath = GetResourcePath(folderToDelete.Id, state.ProjectFilesRoot);
            await fileSystem.DeleteDirectoryIfExistsAsync(deletedDirectoryPath);
        }

        public async Task MoveFolderAsync(ProjectFilesWorkspaceState state, string movedFolderId, string newContainingFolderId)
        {
            WorkspaceDirectory dirToMoveFrom = FindContainingDirectoryState(state, movedFolderId);
            if (dirToMoveFrom == state.Root)
            {
                throw new InvalidOperationException("Cannot move the folder");
            }

            if (newContainingFolderId == dirToMoveFrom.Id)
            {
                return;
            }
            string[] containingFolderPaths = newContainingFolderId.Split('/');
            string[] movedFolderPaths = movedFolderId.Split('/');
            if (movedFolderPaths.Length < containingFolderPaths.Length
                && movedFolderPaths.All((path, i) => containingFolderPaths[i] == path))
            {
                throw new InvalidOperationException("Cannot nest parent directory into child one.");
            }
            WorkspaceDirectory movedDirectory = dirToMoveFrom.Directories.FirstOrDefault(f => f.Id == movedFolderId)
                ?? throw new InvalidOperationException("Moved folder doesn't exist.");

            dirToMoveFrom.Directories.Remove(movedDirectory);

            WorkspaceDirectory newContainingFolder = FindDirectoryState(state, newContainingFolderId);
            bool hasFolderWithSameName = newContainingFolder.Directories.Any(d => d.Name == movedDirectory.Name);
            if (hasFolderWithSameName)
            {
                throw new InvalidOperationException("Folder with this name already exists.");
            }


            string sourcePath = GetResourcePath(movedFolderId, state.ProjectFilesRoot);
            string destinationFolderId = newContainingFolderId + "/" + movedDirectory.Name;
            string destinationPath = GetResourcePath(destinationFolderId, state.ProjectFilesRoot);
            await fileSystem.MoveDirectoryAsync(sourcePath, destinationPath);

            var dirs = fileSystem.ListDirectoryFiles(destinationPath);
            var destinationFolder = MapDirectory(dirs, state.ProjectFilesRoot);

            newContainingFolder.Directories.Add(destinationFolder);
        }
    }
}
