using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Shelngn.Api.Filters;
using Shelngn.Entities;
using Shelngn.Exceptions;
using Shelngn.Repositories;
using Shelngn.Services.GameProjects.Files;

namespace Shelngn.Api.GameProjects.Screenshots
{
    [ApiController]
    [Route("GameProject/screenshot")]
    public class GameProjectScreenshotController : ControllerBase
    {
        private readonly IGameProjectScreenshotRepository gameProjectScreenshotRepository;
        private readonly IProjectFileAccessor projectFileAccessor;
        private readonly IContentTypeProvider contentTypeProvider;
        private readonly IFileSystem fileSystem;

        public GameProjectScreenshotController(
            IGameProjectScreenshotRepository gameProjectScreenshotRepository,
            IProjectFileAccessor projectFileAccessor,
            IFileSystem fileSystem)
        {
            this.gameProjectScreenshotRepository = gameProjectScreenshotRepository;
            this.projectFileAccessor = projectFileAccessor;
            this.contentTypeProvider = new FileExtensionContentTypeProvider();
            this.fileSystem = fileSystem;
        }

        [HttpPost("{gameProjectId}/{*screenshotPath}")]
        [Authorize(GameProjectAuthPolicy.WorkspaceWrite)]
        public async Task<IActionResult> OnScreenshotUploaded(
            [FromRoute] string gameProjectId,
            [FromRoute] string screenshotPath)
        {
            string? screenshotFile = this.projectFileAccessor.GetFilePath(gameProjectId, screenshotPath)
                ?? throw new BadRequestException("Screenshot file does not exist.");

            Guid projectId = Guids.FromUrlSafeBase64(gameProjectId);
            await this.gameProjectScreenshotRepository.CreateScreenshotAsync(new Entities.GameProjectScreenshot
            {
                GameProjectId = projectId,
                ImageUrl = screenshotPath,
            });
            return NoContent();
        }

        [HttpDelete("{gameProjectId}/{screenshotId}")]
        [Authorize(GameProjectAuthPolicy.WorkspaceWrite)]
        public async Task<IActionResult> DeleteScreenshot(
            [FromRoute] string gameProjectId,
            [FromRoute] string screenshotId)
        {
            Guid screenshotIdGuid = Guids.FromUrlSafeBase64(screenshotId);
            GameProjectScreenshot screenshot = await  gameProjectScreenshotRepository.GetByIdAsync(screenshotIdGuid)
                ?? throw new NotFoundException("Game project screenshot");

            string? screenshotPath = projectFileAccessor.GetFilePath(gameProjectId, screenshot.ImageUrl);
            if (screenshotPath != null)
            {
                await fileSystem.DeleteFileIfExistsAsync(screenshotPath);
            }

            await gameProjectScreenshotRepository.DeleteScreenshotAsync(screenshotIdGuid);

            return NoContent();
        }

        [HttpGet("{gameProjectId}/{*screenshotPath}")]
        public IActionResult GetFile(
            [FromRoute] string gameProjectId,
            [FromRoute] string screenshotPath)
        {
            string? screenshotFile = this.projectFileAccessor.GetFilePath(gameProjectId, screenshotPath)
                ?? throw new BadRequestException("Screenshot file does not exist.");

            if (!this.contentTypeProvider.TryGetContentType(screenshotPath, out var contentType))
            {
                return BadRequest();
            }
            var fs = new FileStream(screenshotFile, FileMode.Open, FileAccess.Read);
            return File(fs, contentType);
        }
    }
}
