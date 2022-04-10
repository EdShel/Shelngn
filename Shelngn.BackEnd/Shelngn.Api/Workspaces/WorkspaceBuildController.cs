using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shelngn.Services.GameProjects.Authorization;

namespace Shelngn.Api.Workspaces
{
    [Authorize]
    [ApiController]
    [Route("workspace/build")]
    public class WorkspaceBuildController : ControllerBase
    {
        private readonly IGameProjectAuthorizer gameProjectAuthorizer;

        public WorkspaceBuildController(IGameProjectAuthorizer gameProjectAuthorizer)
        {
            this.gameProjectAuthorizer = gameProjectAuthorizer;
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
            var fs = new FileStream(@"C:\Users\Admin\Desktop\Projects\0dhDWMjAh02BxkIO1cbySg\worker.js", FileMode.Open, FileAccess.Read);
            return File(fs, "text/javascript");
        }
    }
}
