using Shelngn.Models;

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

    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken refreshToken, CancellationToken ct = default);
        Task DeleteAsync(string value, CancellationToken ct = default);
        Task<RefreshToken?> GetByValueAsync(string value, CancellationToken ct = default);
    }
}
