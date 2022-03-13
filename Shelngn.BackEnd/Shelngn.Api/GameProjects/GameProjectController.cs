using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shelngn.Entities;
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
        private readonly IMapper mapper;

        public GameProjectController(
            IGameProjectCreator gameProjectCreator,
            IGameProjectSearcher gameProjectSearcher,
            IMapper mapper)
        {
            this.gameProjectCreator = gameProjectCreator;
            this.gameProjectSearcher = gameProjectSearcher;
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
