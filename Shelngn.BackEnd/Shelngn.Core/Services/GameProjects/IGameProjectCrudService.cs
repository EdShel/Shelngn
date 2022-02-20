using Shelngn.Models;

namespace Shelngn.Services.GameProjects
{
    public interface IFileSystem
    {

        Task CreateFileAsync(Uri path);
    }

    public interface IGameProjectCreator
    {
        Task<GameProject> CreateAsync(GameProject gameProject, Guid appUserId);
    }

    public interface IGameProjectSearcher
    {
        Task<IEnumerable<GameProject>> GetAllForUserAsync(Guid appUserId);
        Task<GameProject> GetByIdAsync(Guid gameProjectId);
    }

    public interface IGameProjectCreatordffd
    {
        Task<GameProject> GetProjectAsync(GameProject gameProject);
        Task<GameProject> UpdateProjectAsync(GameProject gameProject);
        Task<GameProject> DeleteProjectAsync(Guid gameProjectId);
    }
}
