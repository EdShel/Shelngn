using Shelngn.Entities;

namespace Shelngn.Services.GameProjects.Crud
{
    public interface IGameProjectSearcher
    {
        Task<IEnumerable<GameProject>> GetAllForUserAsync(Guid appUserId);
        Task<GameProject?> GetByIdAsync(Guid gameProjectId);
    }
}
