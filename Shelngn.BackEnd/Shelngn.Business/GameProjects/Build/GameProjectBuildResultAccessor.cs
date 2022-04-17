using Shelngn.Services.GameProjects.Build;

namespace Shelngn.Business.GameProjects.Build
{
    public class GameProjectBuildResultAccessor : IGameProjectBuildResultAccessor
    {
        private readonly string projectsDirectory;

        public GameProjectBuildResultAccessor(string projectsDirectory)
        {
            this.projectsDirectory = projectsDirectory;
        }

        public string? GetMainBundle(string gameProjectId)
        {
            string path = Path.Combine(this.projectsDirectory, gameProjectId, "dist/index.js");
            if (!File.Exists(path))
            {
                return null;
            }
            return path;
        }

        public string? GetResource(string gameProjectId, string resourcePath)
        {
            string path = Path.Combine(this.projectsDirectory, gameProjectId, "dist", resourcePath);
            if (!File.Exists(path))
            {
                return null;
            }
            return path;
        }
    }
}
