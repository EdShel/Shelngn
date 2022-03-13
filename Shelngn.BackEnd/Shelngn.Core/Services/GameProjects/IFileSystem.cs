namespace Shelngn.Services.GameProjects
{
    public class ProjectDirectory
    {
        public ProjectDirectory(string name, IEnumerable<ProjectDirectory> directories, IEnumerable<ProjectFile> files)
        {
            this.Name = name;
            this.Directories = directories;
            this.Files = files;
        }

        public string Name { get; set; }
        public IEnumerable<ProjectDirectory> Directories { get; set; }
        public IEnumerable<ProjectFile> Files { get; set; }
    }

    public class ProjectFile
    {
        public ProjectFile(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public Uri? IconUrl { get; }
    }

    public interface IFileSystem
    {
        Task CreateDirectoryOrDoNothingIfExistsAsync(Uri uri);

        Task CreateOrOverwriteFileAsync(Uri uri, byte[] fileContent, CancellationToken ct = default);

        Task<ProjectDirectory?> ListDirectoryFilesAsync(Uri uri, CancellationToken ct = default);

        ProjectDirectory ListDirectoryFiles(string path);
    }
}
