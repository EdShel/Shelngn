using Microsoft.AspNetCore.Authorization;
using Shelngn.Services.GameProjects.Authorization;

namespace Shelngn.Api.Filters
{
    public class GameProjectMemberRequirement : IAuthorizationRequirement
    {
        public GameProjectRights Permissions { get; }

        public GameProjectMemberRequirement(GameProjectRights permissions)
        {
            this.Permissions = permissions;
        }
    }
}
