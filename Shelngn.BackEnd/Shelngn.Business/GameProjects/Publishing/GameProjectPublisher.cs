using Shelngn.Exceptions;
using Shelngn.Repositories;
using Shelngn.Services.GameProjects.Build;
using Shelngn.Services.GameProjects.Publishing;

namespace Shelngn.Business.GameProjects.Publishing
{
    public class GameProjectPublisher : IGameProjectPublisher
    {
        private readonly IGameProjectBuildResultAccessor gameProjectBuildResultAccessor;
        private readonly IGameProjectPublicationRepository gameProjectPublicationRepository;

        public GameProjectPublisher(
            IGameProjectBuildResultAccessor gameProjectBuildResultAccessor,
            IGameProjectPublicationRepository gameProjectPublicationRepository)
        {
            this.gameProjectBuildResultAccessor = gameProjectBuildResultAccessor;
            this.gameProjectPublicationRepository = gameProjectPublicationRepository;
        }

        public async Task PublishGameProjectAsync(Guid gameProjectId)
        {
            string base64 = gameProjectId.ToUrlSafeBase64();
            string? productionBundle = this.gameProjectBuildResultAccessor.GetProductionMainBundle(base64);
            if (productionBundle == null)
            {
                throw new BadRequestException("Production bundle not built yet.");
            }
            if (!await this.gameProjectPublicationRepository.ExistsAsync(gameProjectId))
            {
                await this.gameProjectPublicationRepository.CreatePublicationAsync(new Entities.GameProjectPublication
                {
                    GameProjectId = gameProjectId
                });
            }
        }

        public async Task UnpublishGameProjectAsync(Guid gameProjectId)
        {
            if (!await this.gameProjectPublicationRepository.ExistsAsync(gameProjectId))
            {
                throw new BadRequestException("Not published.");
            }
            await this.gameProjectPublicationRepository.DeletePublicationAsync(gameProjectId);
        }
    }
}
