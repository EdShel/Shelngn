using Shelngn.Entities;

namespace Shelngn.Repositories
{
    public interface IGameProjectPublicationRepository
    {
        Task CreatePublicationAsync(GameProjectPublication publication, CancellationToken ct = default);
        Task DeletePublicationAsync(Guid gameProjectId, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid gameProjectId, CancellationToken ct = default);
    }
}
