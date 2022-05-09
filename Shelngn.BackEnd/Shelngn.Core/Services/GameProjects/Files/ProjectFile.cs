namespace Shelngn.Services.GameProjects.Files
{
    public class ProjectFile
    {
        public ProjectFile(string fullPath)
        {
            this.FullPath = fullPath;
        }

        public string FullPath { get; }
    }
}
