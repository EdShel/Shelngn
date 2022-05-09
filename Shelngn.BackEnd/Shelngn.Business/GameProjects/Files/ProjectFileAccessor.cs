using Shelngn.Exceptions;
using Shelngn.Services.GameProjects.Files;

namespace Shelngn.Business.GameProjects.Files
{
    public class ProjectFileAccessor : IProjectFileAccessor
    {
        private readonly string projectsDirectory;

        public ProjectFileAccessor(string projectsDirectory)
        {
            this.projectsDirectory = projectsDirectory;
        }

        public string? GetFilePath(string workspaceIdGuid, string filePath)
        {
            string[] forbiddenSequences = new[] { "..", ":", "~", "//" };
            if (forbiddenSequences.Any(banned => filePath.Contains(banned)))
            {
                throw new BadRequestException("File path is forbidden.");
            }

            string path = Path.Combine(this.projectsDirectory, workspaceIdGuid, filePath);
            if (!File.Exists(path))
            {
                return null;
            }
            return path;
        }
    }
}
