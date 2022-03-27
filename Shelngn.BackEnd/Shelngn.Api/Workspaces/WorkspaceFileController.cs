using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shelngn.Services.FileUpload;
using Shelngn.Services.GameProjects.Authorization;

namespace Shelngn.Api.Workspaces
{
    [Authorize]
    [ApiController]
    [Route("workspace/file")]
    public class WorkspaceFileController : ControllerBase
    {
        private readonly IFileUploadUrlSigning fileUploadUrlSigning;
        private readonly IGameProjectAuthorizer gameProjectAuthorizer;
        private readonly IConfiguration configuration;

        public WorkspaceFileController(
            IFileUploadUrlSigning fileUploadUrlSigning,
            IConfiguration configuration,
            IGameProjectAuthorizer gameProjectAuthorizer)
        {
            this.fileUploadUrlSigning = fileUploadUrlSigning;
            this.configuration = configuration;
            this.gameProjectAuthorizer = gameProjectAuthorizer;
        }

        [HttpPost("{workspaceId}/{*filePath}")]
        public async Task<IActionResult> GetUploadUrl(
            [FromRoute] string workspaceId,
            [FromRoute] string filePath,
            [FromHeader(Name = "Content-Type")] string contentType)
        {
            Guid workspaceIdGuid = Guids.FromUrlSafeBase64(workspaceId);
            GameProjectRights userRights = await this.gameProjectAuthorizer.GetRightsForUserAsync(User.GetIdGuid(), workspaceIdGuid);
            if (!userRights.Workspace)
            {
                return Forbid();
            }
            string[] forbiddenSequences = new[] { "..", ":", "~", "//" };
            if (forbiddenSequences.Any(banned => filePath.Contains(banned)))
            {
                return BadRequest("Double dots are forbidden.");
            }

            string fileServerPath = $"{workspaceId}/{filePath}" ?? throw new ArgumentNullException("Path can't be null.");
            string signature = fileUploadUrlSigning.CreateSignature(fileServerPath, contentType);
            string fileUploadServer = this.configuration.GetValue<string>("FileUploadServer")
                ?? throw new InvalidOperationException("No file upload server specified.");
            Uri fileUploadSignedUrl = new UriBuilder(fileUploadServer)
            {
                Path = fileServerPath,
                Query = $"sign={signature}"
            }.Uri;
            return Ok(new
            {
                SignedUrl = fileUploadSignedUrl,
                FileId = filePath,
            });
        }
    }
}
