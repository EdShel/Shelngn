namespace Shelngn.Services.Workspaces.ProjectFiles
{
    public interface IProjectFilesWorkspaceState
    {
        string ProjectFilesRoot { get; set; }
        WorkspaceDirectory Root { get; set; }
    }
}