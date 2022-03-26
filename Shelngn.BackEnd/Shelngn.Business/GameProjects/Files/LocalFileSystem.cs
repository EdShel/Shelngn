using Shelngn.Services.GameProjects;
using Shelngn.Services.GameProjects.Files;

namespace Shelngn.Business.GameProjects.Files
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
            ProjectFile[] files = fileNames.Select(f =>
            {
                return new ProjectFile(f);
            }).ToArray();
            string[] directoriesNames = Directory.GetDirectories(path);
            ProjectDirectory[] directories = directoriesNames.Select(d => ListDirectoryFiles(d)).ToArray();
            return new ProjectDirectory(
                fullPath: path,
                directories: directories,
                files: files
            );
        }

        public Task DeleteFileIfExistsAsync(Uri uri)
        {
            string filePath = UriToPath(uri);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return Task.CompletedTask;
        }
        public Task DeleteDirectoryIfExists(Uri uri)
        {
            string directoryPath = UriToPath(uri);
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath);
            }
            return Task.CompletedTask;
        }

        private static string UriToPath(Uri uri)
        {
            return uri.LocalPath;
        }
    }
}
