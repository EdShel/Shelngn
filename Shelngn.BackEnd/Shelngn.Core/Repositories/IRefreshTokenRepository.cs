using Shelngn.Models;

namespace Shelngn.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task CreateAsync(RefreshToken refreshToken, CancellationToken ct = default);
        Task DeleteAsync(string value, CancellationToken ct = default);
        Task<RefreshToken?> GetByValueAsync(string value, CancellationToken ct = default);
    }
}
