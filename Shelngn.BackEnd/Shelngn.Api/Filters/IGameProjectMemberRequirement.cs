using Shelngn.Services.GameProjects.Authorization;

namespace Shelngn.Api.Filters
{
    public interface IGameProjectMemberRequirement
    {
        GameProjectRights Permissions { get; }
    }
}