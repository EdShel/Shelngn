using Shelngn.Entities;

namespace Shelngn.Repositories
{
    public interface IGameProjectRepository
    {
        Task CreateAsync(GameProject gameProject, CancellationToken ct = default);
        Task UpdateNameAsync(Guid gameProjectId, string newProjectName, CancellationToken ct = default);
        Task DeleteAsync(Guid gameProjectId, CancellationToken ct = default);
        Task<IEnumerable<GameProject>> GetAllForUserAsync(Guid appUserId, CancellationToken ct = default);
        Task<GameProject?> GetByIdAsync(Guid gameProjectId, CancellationToken ct = default);
        Task AddMemberAsync(GameProjectMember gameProjectMember, CancellationToken ct = default);
        Task RemoveMemberAsync(Guid gameProjectId, Guid appUserId, CancellationToken ct = default);
        Task<GameProjectMember?> GetMemberAsync(Guid gameProjectId, Guid appUserId, CancellationToken ct = default);
        Task<IEnumerable<GameProjectMemberUser>> GetAllMembersAsync(Guid gameProjectId, CancellationToken ct = default);
    }
}
