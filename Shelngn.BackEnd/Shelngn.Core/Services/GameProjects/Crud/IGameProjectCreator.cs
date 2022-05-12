using Shelngn.Entities;

namespace Shelngn.Services.GameProjects.Crud
{
    public interface IGameProjectCreator
    {
        Task<GameProject> CreateAsync(GameProject gameProject, Guid appUserId);
    }
}
