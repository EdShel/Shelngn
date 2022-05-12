using Shelngn.Entities;
using Shelngn.Repositories;
using Shelngn.Services.GameProjects.Crud;

namespace Shelngn.Business.GameProjects.Crud
{
    public class GameProjectSearcher : IGameProjectSearcher
    {
        private readonly IGameProjectRepository gameProjectRepository;

        public GameProjectSearcher(IGameProjectRepository gameProjectRepository)
        {
            this.gameProjectRepository = gameProjectRepository;
        }

        public async Task<IEnumerable<GameProject>> GetAllForUserAsync(Guid appUserId)
        {
            return await gameProjectRepository.GetAllForUserAsync(appUserId);
        }

        public async Task<GameProject?> GetByIdAsync(Guid gameProjectId)
        {
            return await gameProjectRepository.GetByIdAsync(gameProjectId);
        }
    }
}
