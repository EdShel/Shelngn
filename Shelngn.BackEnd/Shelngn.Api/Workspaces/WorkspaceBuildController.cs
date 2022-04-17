using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
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
        private readonly IContentTypeProvider contentTypeProvider;

        public WorkspaceBuildController(IGameProjectAuthorizer gameProjectAuthorizer, IGameProjectBuildResultAccessor gameProjectBuildResultAccessor)
        {
            this.gameProjectAuthorizer = gameProjectAuthorizer;
            this.gameProjectBuildResultAccessor = gameProjectBuildResultAccessor;
            this.contentTypeProvider = new FileExtensionContentTypeProvider();
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

        [HttpGet("{workspaceId}/{*filePath}")]
        [AllowAnonymous]
        public IActionResult GetFile(
            [FromRoute] string workspaceId,
            [FromRoute] string filePath)
        {
            string[] forbiddenSequences = new[] { "..", ":", "~", "//" };
            if (forbiddenSequences.Any(banned => filePath.Contains(banned)))
            {
                return BadRequest("File path is forbidden.");
            }

            string? filePhysicalPath = gameProjectBuildResultAccessor.GetResource(workspaceId, filePath);
            if (filePhysicalPath == null)
            {
                return NotFound();
            }
            if (!contentTypeProvider.TryGetContentType(filePath, out var contentType))
            {
                return BadRequest();
            }
            var fs = new FileStream(filePhysicalPath, FileMode.Open, FileAccess.Read);
            return File(fs, contentType);
        }
    }
}
