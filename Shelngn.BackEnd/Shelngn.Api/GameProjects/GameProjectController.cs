using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shelngn.Entities;
using Shelngn.Exceptions;
using Shelngn.Repositories;
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
        private readonly IGameProjectRepository gameProjectRepository;
        private readonly IAppUserRepository appUserRepository;
        private readonly IGameProjectDeleter gameProjectDeleter;
        private readonly IMapper mapper;

        public GameProjectController(
            IGameProjectCreator gameProjectCreator,
            IGameProjectSearcher gameProjectSearcher,
            IMapper mapper,
            IGameProjectRepository gameProjectRepository,
            IGameProjectDeleter gameProjectDeleter,
            IAppUserRepository appUserRepository)
        {
            this.gameProjectCreator = gameProjectCreator;
            this.gameProjectSearcher = gameProjectSearcher;
            this.mapper = mapper;
            this.gameProjectRepository = gameProjectRepository;
            this.gameProjectDeleter = gameProjectDeleter;
            this.appUserRepository = appUserRepository;
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

        [HttpGet("{gameProjectId}")]
        [Authorize(Filters.GameProjectAuthPolicy.JustBeingMember)]
        public async Task<IActionResult> GetProjectInfo([FromRoute] string gameProjectId, CancellationToken ct)
        {
            Guid id = Guids.FromUrlSafeBase64(gameProjectId);
            GameProject? gameProject = await gameProjectSearcher.GetByIdAsync(id)
                ?? throw new NotFoundException("Game project");
            IEnumerable<AppUser> usersMembers = await gameProjectRepository.GetAllMembersAsync(id, ct);

            GameProjectViewModel viewModel = mapper.Map<GameProjectViewModel>(gameProject);
            viewModel.Members = mapper.Map<IEnumerable<GameProjectMemberViewModel>>(usersMembers);

            return Ok(viewModel);
        }

        [HttpPost("{gameProjectId}/member")]
        [Authorize(Filters.GameProjectAuthPolicy.ChangeMembers)]
        public async Task<IActionResult> AddMember(
            [FromRoute] string gameProjectId,
            [FromBody] AddMemberModel model,
            CancellationToken ct)
        {
            Guid id = Guids.FromUrlSafeBase64(gameProjectId);
            AppUser? user = await appUserRepository.GetFirstByEmailOrNullAsync(model.EmailOrUserName, ct)
                ?? await appUserRepository.GetFirstByUserNameOrNullAsync(model.EmailOrUserName, ct);
            if (user == null)
            {
                throw new BadRequestException("User does not exist");
            }

            await gameProjectRepository.AddMemberAsync(new GameProjectMember { AppUserId = user.Id, GameProjectId = id }, ct);

            return NoContent();
        }

        [HttpDelete("{gameProjectId}/member/{appUserId}")]
        [Authorize(Filters.GameProjectAuthPolicy.ChangeMembers)]
        public async Task<IActionResult> RemoveMember(
            [FromRoute] string gameProjectId,
            [FromRoute] string appUserId,
            CancellationToken ct)
        {
            Guid projectId = Guids.FromUrlSafeBase64(gameProjectId);
            Guid userId = Guids.FromUrlSafeBase64(appUserId);

            await gameProjectRepository.RemoveMemberAsync(projectId, userId, ct);

            return NoContent();
        }

        [HttpDelete("{gameProjectId}")]
        [Authorize(Filters.GameProjectAuthPolicy.ChangeMembers)]
        public async Task<IActionResult> DeleteProject([FromRoute] string gameProjectId)
        {
            Guid projectId = Guids.FromUrlSafeBase64(gameProjectId);
            await gameProjectDeleter.DeleteProjectAsync(projectId);
            return NoContent();
        }
    }
}
