using Shelngn.Services.GameProjects;

namespace Shelngn.Business.GameProjects
{
    public class LocalFileSystem : IFileSystem
    {
        public Task CreateDirectoryOrDoNothingIfExistsAsync(Uri uri)
        {
            string path = UriToPath(uri);
            Directory.CreateDirectory(path);
            return Task.CompletedTask;
        }

        public async Task CreateOrOverwriteFileAsync(Uri uri, byte[] fileContent, CancellationToken ct = default)
        {
            string path = UriToPath(uri);
            await File.WriteAllBytesAsync(path, fileContent, ct);
        }

        public Task<ProjectDirectory?> ListDirectoryFilesAsync(Uri uri, CancellationToken ct = default)
        {
            string path = UriToPath(uri);
            if (!Directory.Exists(path))
            {
                return Task.FromResult<ProjectDirectory?>(null);
            }
            var root = ListDirectoryFiles(path);
            return Task.FromResult<ProjectDirectory?>(root);
        }

        public ProjectDirectory ListDirectoryFiles(string path)
        {
            string[] fileNames = Directory.GetFiles(path);
            ProjectFile[] files = fileNames.Select(f => new ProjectFile(Path.GetFileName(f))).ToArray();
            string[] directoriesNames = Directory.GetDirectories(path);
            ProjectDirectory[] directories = directoriesNames.Select(d => ListDirectoryFiles(d)).ToArray();
            return new ProjectDirectory(
                name: Path.GetFileName(path)!,
                directories: directories,
                files: files
            );
        }

        private static string UriToPath(Uri uri)
        {
            return uri.LocalPath;
        }
    }
}
