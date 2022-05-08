using Microsoft.Extensions.Options;
using Shelngn.Entities;
using Shelngn.Repositories;
using Shelngn.Services.GameProjects;
using Shelngn.Services.GameProjects.Files;

namespace Shelngn.Business.GameProjects
{
    public class GameProjectCreator : IGameProjectCreator
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepositoryFactory repositoryFactory;
        private readonly IGameProjectStorageBalancer storageBalancer;
        private readonly IFileSystem fileSystem;
        private readonly GameProjectCreateSettings gameProjectCreateSettings;

        public GameProjectCreator(
            IUnitOfWork unitOfWork,
            IRepositoryFactory repositoryFactory,
            IGameProjectStorageBalancer storageBalancer,
            IFileSystem fileSystem,
            IOptions<GameProjectCreateSettings> gameProjectCreateSettings)
        {
            this.unitOfWork = unitOfWork;
            this.repositoryFactory = repositoryFactory;
            this.storageBalancer = storageBalancer;
            this.fileSystem = fileSystem;
            this.gameProjectCreateSettings = gameProjectCreateSettings.Value;
        }

        public async Task<GameProject> CreateAsync(GameProject gameProject, Guid appUserId)
        {
            var gameProjectRepository = this.repositoryFactory.Create<IGameProjectRepository>(this.unitOfWork);

            this.unitOfWork.Begin();
            try
            {
                gameProject.Id = Guid.NewGuid();
                gameProject.FilesLocation = (await this.storageBalancer.RequestNewUriAsync(gameProject.Id)).LocalPath;
                await gameProjectRepository.CreateAsync(gameProject);

                var owner = new GameProjectMember { AppUserId = appUserId, GameProjectId = gameProject.Id, MemberRole = MemberRole.Owner };
                await gameProjectRepository.AddMemberAsync(owner);

                await this.fileSystem.CreateDirectoryOrDoNothingIfExistsAsync(gameProject.FilesLocation);

                string templateFolder = Path.Combine(this.gameProjectCreateSettings.TemplatesDirectory, "Empty");
                await this.fileSystem.CopyDirectoryContentsAsync(templateFolder, gameProject.FilesLocation);

                this.unitOfWork.Commit();

                return gameProject;
            }
            catch (Exception)
            {
                this.unitOfWork.Rollback();
                throw;
            }
        }
    }
}
