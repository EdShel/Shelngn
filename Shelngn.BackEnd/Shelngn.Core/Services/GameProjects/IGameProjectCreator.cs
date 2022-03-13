using Shelngn.Entities;

namespace Shelngn.Services.GameProjects
{
    public interface IGameProjectCreator
    {
        Task<GameProject> CreateAsync(GameProject gameProject, Guid appUserId);
    }
}
