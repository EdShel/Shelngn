using Shelngn.Entities;
using Shelngn.Exceptions;
using Shelngn.Repositories;
using Shelngn.Services.GameProjects;
using Shelngn.Services.GameProjects.Files;

namespace Shelngn.Business.GameProjects
{
    public class GameProjectDeleter : IGameProjectDeleter
    {
        private readonly IRepositoryFactory repositoryFactory;
        private readonly IUnitOfWork unitOfWork;
        private readonly IFileSystem fileSystem;

        public GameProjectDeleter(IRepositoryFactory repositoryFactory, IUnitOfWork unitOfWork, IFileSystem fileSystem)
        {
            this.repositoryFactory = repositoryFactory;
            this.unitOfWork = unitOfWork;
            this.fileSystem = fileSystem;
        }

        public async Task DeleteProjectAsync(Guid gameProjectId)
        {
            var gameProjectRepository = this.repositoryFactory.Create<IGameProjectRepository>(this.unitOfWork);

            this.unitOfWork.Begin();
            try
            {
                GameProject gameProject = await gameProjectRepository.GetByIdAsync(gameProjectId)
                    ?? throw new NotFoundException("Game project");

                await fileSystem.DeleteDirectoryIfExistsAsync(gameProject.FilesLocation);

                await gameProjectRepository.DeleteAsync(gameProjectId);

                this.unitOfWork.Commit();
            }
            catch (Exception)
            {
                this.unitOfWork.Rollback();
                throw;
            }
        }
    }
}
