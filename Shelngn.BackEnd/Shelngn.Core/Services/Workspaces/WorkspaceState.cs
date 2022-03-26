using Shelngn.Services.Workspaces.ActiveUsers;
using Shelngn.Services.Workspaces.ProjectFiles;

namespace Shelngn.Services.Workspaces
{
    public class WorkspaceState
    {
        public WorkspaceState(ActiveUsersWorkspaceState activeUsers, ProjectFilesWorkspaceState projectFiles)
        {
            this.ActiveUsers = activeUsers;
            this.ProjectFiles = projectFiles;
        }

        public ActiveUsersWorkspaceState ActiveUsers { get; private set; }
        public ProjectFilesWorkspaceState ProjectFiles { get; private set; }
    }
}
