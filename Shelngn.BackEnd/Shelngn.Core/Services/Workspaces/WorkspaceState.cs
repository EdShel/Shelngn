using Shelngn.Services.Workspaces.ActiveUsers;
using Shelngn.Services.Workspaces.ProjectFiles;

namespace Shelngn.Services.Workspaces
{
    public class WorkspaceState
    {
        public WorkspaceState(IActiveUsersWorkspaceState activeUsers, IProjectFilesWorkspaceState projectFiles)
        {
            this.ActiveUsers = activeUsers;
            this.ProjectFiles = projectFiles;
        }

        public IActiveUsersWorkspaceState ActiveUsers { get; private set; }
        public IProjectFilesWorkspaceState ProjectFiles { get; private set; }
    }
}
