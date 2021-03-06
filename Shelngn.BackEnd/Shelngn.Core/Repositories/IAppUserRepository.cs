using Shelngn.Entities;

namespace Shelngn.Repositories
{
    public interface IAppUserRepository
    {
        Task CreateAsync(AppUser user, CancellationToken ct = default);
        Task UpdateAllAsync(AppUser user, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<AppUser?> GetFirstByIdOrNullAsync(Guid id, CancellationToken ct = default);
        Task<AppUser?> GetFirstByUserNameOrNullAsync(string userName, CancellationToken ct = default);
        Task<AppUser?> GetFirstByEmailOrNullAsync(string email, CancellationToken ct = default);
    }
}
