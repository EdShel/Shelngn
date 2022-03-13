using Shelngn.Repositories;
using Shelngn.Services.GameProjects;

namespace Shelngn.Business.GameProjects
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
