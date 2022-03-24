using Shelngn.Repositories;

namespace Shelngn.Services.GameProjects.Authorization
{
    public class GameProjectAuthorizer : IGameProjectAuthorizer
    {
        private readonly IGameProjectRepository gameProjectRepository;

        public GameProjectAuthorizer(IGameProjectRepository gameProjectRepository)
        {
            this.gameProjectRepository = gameProjectRepository;
        }

        public async Task<GameProjectRights> GetRightsForUserAsync(Guid userId, Guid gameProjectId)
        {
            bool isUserProjectMember = await this.gameProjectRepository.GetMemberAsync(gameProjectId, userId) != null;
            return new GameProjectRights
            {
                Workspace = isUserProjectMember,
            };
        }
    }
}
