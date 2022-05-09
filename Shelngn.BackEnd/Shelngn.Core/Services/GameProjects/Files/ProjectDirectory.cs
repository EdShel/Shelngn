namespace Shelngn.Services.GameProjects.Files
{
    public class ProjectDirectory
    {
        public ProjectDirectory(string fullPath, IEnumerable<ProjectDirectory> directories, IEnumerable<ProjectFile> files)
        {
            this.FullPath = fullPath;
            this.Directories = directories;
            this.Files = files;
        }

        public string FullPath { get; set; }
        public IEnumerable<ProjectDirectory> Directories { get; set; }
        public IEnumerable<ProjectFile> Files { get; set; }
    }
}
