namespace Shelngn.Services.GameProjects.Authorization
{
    public interface IGameProjectAuthorizer
    {
        Task<GameProjectRights> GetRightsForUserAsync(Guid userId, Guid gameProjectId);
    }
}
