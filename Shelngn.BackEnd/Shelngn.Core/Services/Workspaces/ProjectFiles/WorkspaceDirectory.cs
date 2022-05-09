namespace Shelngn.Services.Workspaces.ProjectFiles
{
    public class WorkspaceDirectory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<WorkspaceDirectory> Directories { get; set; }
        public IList<WorkspaceFile> Files { get; set; }
    }
}
