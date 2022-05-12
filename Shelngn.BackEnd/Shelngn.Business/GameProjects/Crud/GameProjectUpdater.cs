using Shelngn.Repositories;
using Shelngn.Services.GameProjects.Crud;

namespace Shelngn.Business.GameProjects.Crud
{
    public class GameProjectUpdater : IGameProjectUpdater
    {
        private readonly IGameProjectRepository gameProjectRepository;

        public GameProjectUpdater(IGameProjectRepository gameProjectRepository)
        {
            this.gameProjectRepository = gameProjectRepository;
        }

        public async Task UpdateNameAsync(Guid gameProjectId, string newProjectName, CancellationToken ct = default)
        {
            await gameProjectRepository.UpdateNameAsync(gameProjectId, newProjectName, ct);
        }
    }
}
