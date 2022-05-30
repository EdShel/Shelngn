namespace Shelngn.Services.Workspaces.ProjectFiles
{
    public interface IProjectFilesWorkspaceStateReducer
    {
        Task<IProjectFilesWorkspaceState> GetInitialStateAsync(Guid workspaceId);
        Task CreateEmptyFileAsync(IProjectFilesWorkspaceState state, string folderId, string fileName);
        Task CreateFolderAsync(IProjectFilesWorkspaceState state, string containingFolderId, string folderName);
        Task DeleteFileAsync(IProjectFilesWorkspaceState state, string fileId);
        Task DeleteFolderAsync(IProjectFilesWorkspaceState state, string folderId);
        Task FileUploadedAsync(IProjectFilesWorkspaceState state, Guid workspaceId);
        Task MoveFileAsync(IProjectFilesWorkspaceState state, string fileId, string folderId);
        Task MoveFolderAsync(IProjectFilesWorkspaceState state, string movedFolderId, string newContainingFolderId);
        string GetResourcePath(string id, string workspaceFolder);
    }
}
