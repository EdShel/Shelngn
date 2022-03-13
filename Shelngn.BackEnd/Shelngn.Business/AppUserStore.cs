using Shelngn.Entities;
using Shelngn.Repositories;
using Shelngn.Services;

namespace Shelngn.Business
{
    public class AppUserStore : IAppUserStore, IDisposable
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepositoryFactory repositoryFactory;
        private readonly IAppUserRepository appUserRepository;

        public AppUserStore(IRepositoryFactory repositoryFactory, IUnitOfWork unitOfWork)
        {
            this.repositoryFactory = repositoryFactory;
            this.unitOfWork = unitOfWork;
            this.appUserRepository = this.repositoryFactory.Create<IAppUserRepository>(this.unitOfWork);
        }

        public async Task<AppUser> CreateAsync(AppUser user, CancellationToken cancellationToken = default)
        {
            user.Id = Guid.NewGuid();
            user.UserName ??= user.Id.ToUrlSafeBase64();
            await this.appUserRepository.CreateAsync(user, cancellationToken);

            return user;
        }

        public async Task DeleteAsync(AppUser user, CancellationToken cancellationToken = default)
        {
            await this.appUserRepository.DeleteAsync(user.Id, cancellationToken);
        }

        public async Task UpdateAsync(AppUser user, CancellationToken cancellationToken = default)
        {
            await this.appUserRepository.UpdateAllAsync(user, cancellationToken);
        }

        public async Task<AppUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            Guid id = Guid.Parse(userId);

            return await this.appUserRepository.GetFirstByIdOrNullAsync(id, cancellationToken);
        }

        public async Task<AppUser?> FindByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await this.appUserRepository.GetFirstByIdOrNullAsync(userId, cancellationToken);
        }

        public async Task<AppUser?> FindByNameAsync(string userName, CancellationToken cancellationToken = default)
        {
            return await this.appUserRepository.GetFirstByUserNameOrNullAsync(userName, cancellationToken);
        }

        public async Task<AppUser?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await this.appUserRepository.GetFirstByEmailOrNullAsync(email, cancellationToken);
        }

        void IDisposable.Dispose()
        {
            this.unitOfWork.Dispose();
        }
    }
}
