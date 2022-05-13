using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Shelngn.Api.Filters;
using Shelngn.Services.FileUpload;
using Shelngn.Services.GameProjects.Authorization;
using Shelngn.Services.GameProjects.Files;

namespace Shelngn.Api.Workspaces
{
    [Authorize]
    [ApiController]
    [Route("workspace/file")]
    public class WorkspaceFileController : ControllerBase
    {
        private readonly IFileUploadUrlSigning fileUploadUrlSigning;
        private readonly IProjectFileAccessor projectFileAccessor;
        private readonly IConfiguration configuration;
        private readonly IContentTypeProvider contentTypeProvider;

        public WorkspaceFileController(
            IFileUploadUrlSigning fileUploadUrlSigning,
            IConfiguration configuration,
            IProjectFileAccessor projectFileAccessor,
            IContentTypeProvider contentTypeProvider)
        {
            this.fileUploadUrlSigning = fileUploadUrlSigning;
            this.configuration = configuration;
            this.projectFileAccessor = projectFileAccessor;
            this.contentTypeProvider = contentTypeProvider;
        }

        [HttpGet("{gameProjectId}/{*filePath}")]
        [Authorize(GameProjectAuthPolicy.WorkspaceWrite)]
        public IActionResult GetSourceFile(
            [FromRoute] string gameProjectId,
            [FromRoute] string filePath)
        {
            string? filePhysicalPath = projectFileAccessor.GetFilePath(gameProjectId, filePath);
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

        [HttpPost("{gameProjectId}/{*filePath}")]
        [Authorize(GameProjectAuthPolicy.WorkspaceWrite)]
        public IActionResult GetUploadUrl(
            [FromRoute] string gameProjectId,
            [FromRoute] string filePath,
            [FromHeader(Name = "Content-Type")] string contentType)
        {
            string[] forbiddenSequences = new[] { "..", ":", "~", "//" };
                if (forbiddenSequences.Any(banned => filePath.Contains(banned)))
            {
                return BadRequest("File path is forbidden.");
            }

            string fileServerPath = $"{gameProjectId}/{filePath}" ?? throw new ArgumentNullException("Path can't be null.");
            string signature = this.fileUploadUrlSigning.CreateSignature(fileServerPath, contentType);
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
