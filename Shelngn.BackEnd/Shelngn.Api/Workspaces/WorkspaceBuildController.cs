using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Shelngn.Api.Filters;
using Shelngn.Exceptions;
using Shelngn.Services.GameProjects.Build;

namespace Shelngn.Api.Workspaces
{
    [Authorize]
    [ApiController]
    [Route("workspace/build")]
    public class WorkspaceBuildController : ControllerBase
    {
        private readonly IGameProjectBuildResultAccessor gameProjectBuildResultAccessor;
        private readonly IContentTypeProvider contentTypeProvider;

        public WorkspaceBuildController(IGameProjectBuildResultAccessor gameProjectBuildResultAccessor)
        {
            this.gameProjectBuildResultAccessor = gameProjectBuildResultAccessor;
            this.contentTypeProvider = new FileExtensionContentTypeProvider();
        }

        [HttpGet("{gameProjectId}/bundle.js")]
        [Authorize(GameProjectAuthPolicy.WorkspaceWrite)]
        public IActionResult GetBuildJsBundle(
            [FromRoute] string gameProjectId)
        {
            string? bundle = this.gameProjectBuildResultAccessor.GetDebugMainBundle(gameProjectId)
                ?? throw new BadRequestException("Not built");

            var fs = new FileStream(bundle, FileMode.Open, FileAccess.Read);
            return File(fs, "text/javascript");
        }

        [HttpGet("{workspaceId}/{*filePath}")]
        [AllowAnonymous]
        public IActionResult GetFile(
            [FromRoute] string workspaceId,
            [FromRoute] string filePath)
        {
            string? filePhysicalPath = this.gameProjectBuildResultAccessor.GetDebugResource(workspaceId, filePath)
                ?? throw new NotFoundException("Resource file");

            if (!this.contentTypeProvider.TryGetContentType(filePath, out var contentType))
            {
                return BadRequest();
            }
            var fs = new FileStream(filePhysicalPath, FileMode.Open, FileAccess.Read);
            return File(fs, contentType);
        }
    }
}
