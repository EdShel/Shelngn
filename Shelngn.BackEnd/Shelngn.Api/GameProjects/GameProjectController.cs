using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shelngn.Business.GameProjects;
using Shelngn.Exceptions;
using Shelngn.Models;
using Shelngn.Services.GameProjects;

namespace Shelngn.Api.GameProjects
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class GameProjectController : ControllerBase
    {
        private readonly IGameProjectCreator gameProjectCreator;
        private readonly IGameProjectSearcher gameProjectSearcher;
        private readonly LocalFileSystem localFileSystem;
        private readonly IMapper mapper;

        public GameProjectController(
            IGameProjectCreator gameProjectCreator,
            IGameProjectSearcher gameProjectSearcher,
            LocalFileSystem localFileSystem,
            IMapper mapper)
        {
            this.gameProjectCreator = gameProjectCreator;
            this.gameProjectSearcher = gameProjectSearcher;
            this.localFileSystem = localFileSystem;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGameProject()
        {
            var gameProject = new GameProject
            {
                ProjectName = "Unnamed project"
            };
            var creatorId = this.User.GetIdGuid();
            gameProject = await this.gameProjectCreator.CreateAsync(gameProject, creatorId);

            return Ok(new
            {
                Id = gameProject.Id
            });
        }

        [HttpGet("my")]
        public async Task<IActionResult> ListGameProjects()
        {
            Guid userId = this.User.GetIdGuid();
            IEnumerable<GameProject> myProjects = await this.gameProjectSearcher.GetAllForUserAsync(userId);
            return Ok(this.mapper.Map<GameProjectListModel>(myProjects));
        }

        [HttpGet("{id}/ls")]
        public async Task<IActionResult> ListDirTree([FromRoute(Name = "id")] Guid gameProjectId)
        {
            GameProject? gameProject = await this.gameProjectSearcher.GetByIdAsync(gameProjectId)
                ?? throw new NotFoundException("Game project");
            ProjectDirectory files = await this.localFileSystem.ListDirectoryFilesAsync(new Uri(gameProject.FilesLocation))
                ?? throw new NotFoundException("Project directory");
            return Ok(files);
        }
    }

    public record GameProjectListModel(Guid Id, string Name);
}
