using Shelngn.Services.GameProjects.Files;

namespace Shelngn.Business.GameProjects.Files
{
    public class LocalFileSystem : IFileSystem
    {
        public Task CreateDirectoryOrDoNothingIfExistsAsync(string uri)
        {
            Directory.CreateDirectory(uri);
            return Task.CompletedTask;
        }

        public Task CopyDirectoryContentsAsync(string sourcePath, string destinationPath)
        {
            CopyFilesRecursively(new DirectoryInfo(sourcePath), new DirectoryInfo(destinationPath));
            return Task.CompletedTask;
        }

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            }

            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name));
            }
        }

        public async Task CreateOrOverwriteFileAsync(string uri, byte[] fileContent, CancellationToken ct = default)
        {
            using (var fs = new FileStream(uri, FileMode.Create, FileAccess.Write))
            {
                await fs.WriteAsync(fileContent, ct);
            }
        }

        public Task CreateEmptyFileAsync(string path)
        {
            using (File.Create(path)) { }
            return Task.CompletedTask;
        }

        public Task<ProjectDirectory?> ListDirectoryFilesAsync(string uri, CancellationToken ct = default)
        {
            if (!Directory.Exists(uri))
            {
                return Task.FromResult<ProjectDirectory?>(null);
            }
            var root = ListDirectoryFiles(uri);
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

        public Task DeleteFileIfExistsAsync(string uri)
        {
            if (File.Exists(uri))
            {
                File.Delete(uri);
            }
            return Task.CompletedTask;
        }
        public Task DeleteDirectoryIfExistsAsync(string uri)
        {
            if (Directory.Exists(uri))
            {
                Directory.Delete(uri, true);
            }
            return Task.CompletedTask;
        }

        public Task MoveFileAsync(string sourceUri, string destinationUri)
        {
            File.Move(sourceUri, destinationUri, overwrite: true);
            return Task.CompletedTask;
        }

        public Task MoveDirectoryAsync(string sourcePath, string destinationPath)
        {
            Directory.Move(sourcePath, destinationPath);
            return Task.CompletedTask;
        }

        public Task<byte[]> ReadFileAsync(string uri, CancellationToken ct = default)
        {
            return File.ReadAllBytesAsync(uri, ct);
        }
    }
}
