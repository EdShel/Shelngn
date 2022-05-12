using Shelngn.Exceptions;
using Shelngn.Services.GameProjects.Build;

namespace Shelngn.Business.GameProjects.Build
{
    public class GameProjectBuildResultAccessor : IGameProjectBuildResultAccessor
    {
        private const string DEBUG_BUILD_FOLDER = "dist";
        private const string PRODUCTION_BUILD_FOLDER = "publ";

        private readonly string projectsDirectory;

        public GameProjectBuildResultAccessor(string projectsDirectory)
        {
            this.projectsDirectory = projectsDirectory;
        }

        public string? GetDebugMainBundle(string gameProjectId)
        {
            string path = Path.Combine(this.projectsDirectory, gameProjectId, DEBUG_BUILD_FOLDER, "index.js");
            if (!File.Exists(path))
            {
                return null;
            }
            return path;
        }

        public string? GetProductionMainBundle(string gameProjectId)
        {
            string path = Path.Combine(this.projectsDirectory, gameProjectId, PRODUCTION_BUILD_FOLDER, "index.js");
            if (!File.Exists(path))
            {
                return null;
            }
            return path;
        }

        public string? GetDebugResource(string gameProjectId, string resourcePath)
        {
            EnsurePathIsValid(resourcePath);
            string path = Path.Combine(this.projectsDirectory, gameProjectId, DEBUG_BUILD_FOLDER, resourcePath);
            if (!File.Exists(path))
            {
                return null;
            }
            return path;
        }

        private static void EnsurePathIsValid(string filePath)
        {
            string[] forbiddenSequences = new[] { "..", ":", "~", "//" };
            if (forbiddenSequences.Any(banned => filePath.Contains(banned)))
            {
                throw new BadRequestException("File path is forbidden.");
            }
        }

        public string? GetProductionResource(string gameProjectId, string resourcePath)
        {
            EnsurePathIsValid(resourcePath);
            string path = Path.Combine(this.projectsDirectory, gameProjectId, PRODUCTION_BUILD_FOLDER, resourcePath);
            if (!File.Exists(path))
            {
                return null;
            }
            return path;
        }
    }
}
