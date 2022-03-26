namespace Shelngn.Services.GameProjects.Files
{
    public class ProjectDirectory
    {
        public ProjectDirectory(string fullPath, IEnumerable<ProjectDirectory> directories, IEnumerable<ProjectFile> files)
        {
            this.FullPath = fullPath;
            this.Directories = directories;
            this.Files = files;
        }

        public string FullPath { get; set; }
        public IEnumerable<ProjectDirectory> Directories { get; set; }
        public IEnumerable<ProjectFile> Files { get; set; }
    }

    public class ProjectFile
    {
        public ProjectFile(string fullPath)
        {
            this.FullPath = fullPath;
        }

        public string FullPath { get; }
    }

    public interface IFileSystem
    {
        Task CreateDirectoryOrDoNothingIfExistsAsync(Uri uri);

        Task CreateOrOverwriteFileAsync(Uri uri, byte[] fileContent, CancellationToken ct = default);

        Task<ProjectDirectory?> ListDirectoryFilesAsync(Uri uri, CancellationToken ct = default);

        ProjectDirectory ListDirectoryFiles(string path);

        Task DeleteFileIfExistsAsync(Uri uri);

        Task DeleteDirectoryIfExists(Uri uri);
    }
}
