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
                Id = gameProject.Id.ToUrlSafeBase64()
            });
        }

        [HttpGet("my")]
        public async Task<IActionResult> ListGameProjects()
        {
            Guid userId = this.User.GetIdGuid();
            IEnumerable<GameProject> myProjects = await this.gameProjectSearcher.GetAllForUserAsync(userId);
            IEnumerable<GameProjectListModel>? myProjectsModel = this.mapper.Map<IEnumerable<GameProjectListModel>>(myProjects);
            return Ok(new
            {
                GameProjects = myProjectsModel
            });
        }

        [HttpGet("{id}/ls")]
        public async Task<IActionResult> ListDirTree([FromRoute(Name = "id")] string gameProjectBase64Id)
        {
            Guid gameProjectId = GuidExtensions.FromUrlSafeBase64(gameProjectBase64Id);
            GameProject? gameProject = await this.gameProjectSearcher.GetByIdAsync(gameProjectId)
                ?? throw new NotFoundException("Game project");
            ProjectDirectory files = await this.localFileSystem.ListDirectoryFilesAsync(new Uri(gameProject.FilesLocation))
                ?? throw new NotFoundException("Project directory");
            return Ok(files);
        }
    }

    public class GameProjectListModel
    {
        public string Id { get; set; } = null!;
        public string ProjectName { get; set; } = null!;

        private class GameProjectListModelProfile : Profile
        {
            public GameProjectListModelProfile()
            {
                CreateMap<GameProject, GameProjectListModel>()
                    .ForMember(m => m.Id, opt => opt.MapFrom(g => g.Id.ToUrlSafeBase64()));
            }
        }
    }

}
