using Shelngn.Services.Workspaces.ActiveUsers;
using Shelngn.Services.Workspaces.ProjectFiles;

namespace Shelngn.Services.Workspaces
{
    public class WorkspaceState
    {
        public ActiveUsersWorkspaceState ActiveUsers { get; private set; } = new();
        public ProjectFilesWorkspaceState ProjectFiles { get; private set; } = new();
    }
}
