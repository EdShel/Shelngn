using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shelngn.Services.GameProjects.Authorization;
using Shelngn.Services.GameProjects.Build;

namespace Shelngn.Api.Workspaces
{
    [Authorize]
    [ApiController]
    [Route("workspace/build")]
    public class WorkspaceBuildController : ControllerBase
    {
        private readonly IGameProjectAuthorizer gameProjectAuthorizer;
        private readonly IGameProjectBuildResultAccessor gameProjectBuildResultAccessor;

        public WorkspaceBuildController(IGameProjectAuthorizer gameProjectAuthorizer, IGameProjectBuildResultAccessor gameProjectBuildResultAccessor)
        {
            this.gameProjectAuthorizer = gameProjectAuthorizer;
            this.gameProjectBuildResultAccessor = gameProjectBuildResultAccessor;
        }

        [HttpGet("{workspaceId}/bundle.js")]
        public async Task<IActionResult> GetBuildJsBundle(
            [FromRoute] string workspaceId)
        {
            Guid workspaceIdGuid = Guids.FromUrlSafeBase64(workspaceId);
            GameProjectRights userRights = await this.gameProjectAuthorizer.GetRightsForUserAsync(User.GetIdGuid(), workspaceIdGuid);
            if (!userRights.Workspace)
            {
                return Forbid();
            }
            string? bundle = gameProjectBuildResultAccessor.GetMainBundle(workspaceId);
            if (bundle == null)
            {
                return BadRequest(new { error = "Not built" });
            }

            var fs = new FileStream(bundle, FileMode.Open, FileAccess.Read);
            return File(fs, "text/javascript");
        }
    }
}
