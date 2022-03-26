using Shelngn.Entities;
using Shelngn.Repositories;
using Shelngn.Services.GameProjects;
using Shelngn.Services.GameProjects.Files;
using System.Text;

namespace Shelngn.Business.GameProjects
{
    public class GameProjectCreator : IGameProjectCreator
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepositoryFactory repositoryFactory;
        private readonly IGameProjectStorageBalancer storageBalancer;
        private readonly IFileSystem fileSystem;

        public GameProjectCreator(
            IUnitOfWork unitOfWork,
            IRepositoryFactory repositoryFactory,
            IGameProjectStorageBalancer storageBalancer,
            IFileSystem fileSystem)
        {
            this.unitOfWork = unitOfWork;
            this.repositoryFactory = repositoryFactory;
            this.storageBalancer = storageBalancer;
            this.fileSystem = fileSystem;
        }

        public async Task<GameProject> CreateAsync(GameProject gameProject, Guid appUserId)
        {
            var gameProjectRepository = this.repositoryFactory.Create<IGameProjectRepository>(this.unitOfWork);

            this.unitOfWork.Begin();
            try
            {
                gameProject.Id = Guid.NewGuid();
                gameProject.FilesLocation = (await this.storageBalancer.RequestNewUriAsync(gameProject.Id)).ToString();
                await gameProjectRepository.CreateAsync(gameProject);

                var owner = new GameProjectMember { AppUserId = appUserId, GameProjectId = gameProject.Id };
                await gameProjectRepository.AddMemberAsync(owner);

                await this.fileSystem.CreateDirectoryOrDoNothingIfExistsAsync(new Uri(gameProject.FilesLocation));

                var readmeFileUrl = new Uri(Path.Combine(gameProject.FilesLocation, "readme.txt"));
                var readmeFileContent = "TODO: it's just a placeholder";
                await this.fileSystem.CreateOrOverwriteFileAsync(readmeFileUrl, Encoding.UTF8.GetBytes(readmeFileContent));

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
