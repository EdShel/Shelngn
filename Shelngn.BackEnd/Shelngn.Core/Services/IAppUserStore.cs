using Shelngn.Entities;

namespace Shelngn.Services
{
    public interface IAppUserStore
    {
        Task<AppUser> CreateAsync(AppUser user, CancellationToken cancellationToken = default);

        Task UpdateAsync(AppUser user, CancellationToken cancellationToken = default);

        Task DeleteAsync(AppUser user, CancellationToken cancellationToken = default);

        Task<AppUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<AppUser?> FindByIdAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<AppUser?> FindByNameAsync(string userName, CancellationToken cancellationToken = default);

        Task<AppUser?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
