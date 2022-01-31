using Microsoft.Extensions.DependencyInjection;

namespace Shelngn.Repositories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider serviceProvider;

        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public T Create<T>(IUnitOfWork unitOfWork)
            where T : class
        {
            T requestedRepository = this.serviceProvider.GetService<T>()
                ?? throw new InvalidOperationException("Repository implementation is not registered in DI container.");

            BaseRepository asBaseRepository = requestedRepository as BaseRepository
                ?? throw new InvalidOperationException($"The created repository should inherit from {nameof(BaseRepository)}.");
            asBaseRepository.Initialize(unitOfWork);

            return requestedRepository;
        }
    }
}
