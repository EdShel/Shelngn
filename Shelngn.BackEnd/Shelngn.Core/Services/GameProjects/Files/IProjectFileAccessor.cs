namespace Shelngn.Services.GameProjects.Files
{
    public interface IProjectFileAccessor
    {
        string? GetFilePath(string workspaceIdGuid, string filePath);
    }
}
