namespace Shelngn.Services.GameProjects.Files
{
    public interface IFileSystem
    {
        Task CreateDirectoryOrDoNothingIfExistsAsync(string uri);

        Task CopyDirectoryContentsAsync(string sourcePath, string destinationPath);

        Task CreateEmptyFileAsync(string path);

        Task CreateOrOverwriteFileAsync(string uri, byte[] fileContent, CancellationToken ct = default);

        Task<ProjectDirectory?> ListDirectoryFilesAsync(string uri, CancellationToken ct = default);

        ProjectDirectory ListDirectoryFiles(string path);

        Task DeleteFileIfExistsAsync(string uri);

        Task DeleteDirectoryIfExistsAsync(string uri);

        Task MoveFileAsync(string sourceUri, string destinationUri);

        Task MoveDirectoryAsync(string sourcePath, string destinationPath);

        Task<byte[]> ReadFileAsync(string uri, CancellationToken ct = default);
    }
}
