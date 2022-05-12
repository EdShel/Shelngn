using Shelngn.Entities;
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
            GameProjectMember? member = await this.gameProjectRepository.GetMemberAsync(gameProjectId, userId);
            if (member == null)
            {
                return GameProjectRights.NoRights();
            }

            return new GameProjectRights
            {
                Workspace = true,
                ChangeMembers = member.MemberRole == MemberRole.Owner,
                Publishing = member.MemberRole == MemberRole.Owner
            };
        }
    }
}
