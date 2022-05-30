using Shelngn.Services.Workspaces.ProjectFiles;

namespace Shelngn.Business.Workspaces.ProjectFiles
{
    public class ProjectFilesWorkspaceState : IProjectFilesWorkspaceState
    {
        public string ProjectFilesRoot { get; set; } = null!;
        public WorkspaceDirectory Root { get; set; } = null!;
    }
}
