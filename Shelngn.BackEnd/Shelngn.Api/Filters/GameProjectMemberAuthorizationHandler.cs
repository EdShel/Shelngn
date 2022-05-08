using Microsoft.AspNetCore.Authorization;
using Shelngn.Services.GameProjects.Authorization;

namespace Shelngn.Api.Filters
{
    public class GameProjectMemberAuthorizationHandler : AuthorizationHandler<GameProjectMemberRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IGameProjectAuthorizer gameProjectAuthorizer;

        public GameProjectMemberAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IGameProjectAuthorizer gameProjectAuthorizer)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.gameProjectAuthorizer = gameProjectAuthorizer;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, GameProjectMemberRequirement requirement)
        {
            RouteValueDictionary route = httpContextAccessor.HttpContext?.Request.RouteValues
                ?? throw new InvalidOperationException("Calling outside of http context scope.");
            if (!route.TryGetValue("gameProjectId", out object? gameProjectIdObj))
            {
                throw new InvalidOperationException("Calling without 'gameProjectId' route parameter defined.");
            }
            string gameProjectIdString = gameProjectIdObj as string
                ?? throw new InvalidOperationException($"Game project id doesn't seem to be a string but rather {gameProjectIdObj!.GetType().Name}.");
            Guid gameProjectId = Guids.FromUrlSafeBase64(gameProjectIdString);
            Guid currentUserId = context.User.GetIdGuid();

            GameProjectRights userPermissions = await gameProjectAuthorizer.GetRightsForUserAsync(currentUserId, gameProjectId);
            if (requirement.Permissions.DoesOtherObjectSatisfy(userPermissions))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail(new AuthorizationFailureReason(this, "Not all permissions are satisfied."));
            }
        }
    }
}
