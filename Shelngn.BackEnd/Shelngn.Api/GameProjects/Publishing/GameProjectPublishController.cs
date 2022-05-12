using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shelngn.Api.Filters;
using Shelngn.Exceptions;
using Shelngn.Services.GameProjects.Build;
using Shelngn.Services.GameProjects.Publishing;

namespace Shelngn.Api.GameProjects.Publish
{
    [Authorize]
    [ApiController]
    [Route("GameProject/publish")]
    public class GameProjectPublishController : ControllerBase
    {
        private readonly IGameProjectBuilder gameProjectBuilder;
        private readonly IGameProjectPublisher gameProjectPublisher;

        public GameProjectPublishController(IGameProjectBuilder gameProjectBuilder, IGameProjectPublisher gameProjectPublisher)
        {
            this.gameProjectBuilder = gameProjectBuilder;
            this.gameProjectPublisher = gameProjectPublisher;
        }

        [HttpPost("{gameProjectId}")]
        [Authorize(GameProjectAuthPolicy.Publishing)]
        public async Task<IActionResult> PublishGameProject([FromRoute] string gameProjectId)
        {
            BuildResult buildResult = await gameProjectBuilder.BuildProductionProjectAsync(gameProjectId);
            if (!buildResult.IsSuccess)
            {
                throw new BadRequestException("Can't build game project.");
            }

            Guid projectId = Guids.FromUrlSafeBase64(gameProjectId);
            await gameProjectPublisher.PublishGameProjectAsync(projectId);

            return NoContent();
        }

        [HttpDelete("{gameProjectId}")]
        [Authorize(GameProjectAuthPolicy.Publishing)]
        public async Task<IActionResult> UnpublishGameProject([FromRoute] string gameProjectId)
        {
            Guid projectId = Guids.FromUrlSafeBase64(gameProjectId);
            await gameProjectPublisher.UnpublishGameProjectAsync(projectId);
            return NoContent();
        }
    }
}
