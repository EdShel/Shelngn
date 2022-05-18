using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shelngn.Entities;
using Shelngn.Exceptions;
using Shelngn.Repositories;
using Shelngn.Services.GameProjects.Authorization;
using Shelngn.Services.GameProjects.Crud;

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
        private readonly IGameProjectAuthorizer gameProjectAuthorizer;
        private readonly IGameProjectPublicationRepository gameProjectPublicationRepository;
        private readonly IGameProjectScreenshotRepository gameProjectScreenshotRepository;
        private readonly IMapper mapper;

        public GameProjectController(
            IGameProjectCreator gameProjectCreator,
            IGameProjectSearcher gameProjectSearcher,
            IMapper mapper,
            IGameProjectRepository gameProjectRepository,
            IGameProjectDeleter gameProjectDeleter,
            IAppUserRepository appUserRepository,
            IGameProjectAuthorizer gameProjectAuthorizer,
            IGameProjectPublicationRepository gameProjectPublicationRepository,
            IGameProjectScreenshotRepository gameProjectScreenshotRepository)
        {
            this.gameProjectCreator = gameProjectCreator;
            this.gameProjectSearcher = gameProjectSearcher;
            this.mapper = mapper;
            this.gameProjectRepository = gameProjectRepository;
            this.gameProjectDeleter = gameProjectDeleter;
            this.appUserRepository = appUserRepository;
            this.gameProjectAuthorizer = gameProjectAuthorizer;
            this.gameProjectPublicationRepository = gameProjectPublicationRepository;
            this.gameProjectScreenshotRepository = gameProjectScreenshotRepository;
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
                Data = myProjectsModel
            });
        }

        [HttpGet("{gameProjectId}")]
        [Authorize(Filters.GameProjectAuthPolicy.JustBeingMember)]
        public async Task<IActionResult> GetProjectInfo([FromRoute] string gameProjectId, CancellationToken ct)
        {
            Guid currentUserId = this.User.GetIdGuid();
            Guid projectId = Guids.FromUrlSafeBase64(gameProjectId);
            GameProject? gameProject = await this.gameProjectSearcher.GetByIdAsync(projectId)
                ?? throw new NotFoundException("Game project");
            IEnumerable<GameProjectMemberUser> usersMembers = await this.gameProjectRepository.GetAllMembersAsync(projectId, ct);
            IEnumerable<GameProjectScreenshot> screenshots = await this.gameProjectScreenshotRepository.GetAllForProjectAsync(projectId, ct);

            GameProjectViewModel viewModel = this.mapper.Map<GameProjectViewModel>(gameProject);
            viewModel.IsPublished = await gameProjectPublicationRepository.ExistsAsync(projectId, ct);
            viewModel.Members = this.mapper.Map<IEnumerable<GameProjectMemberViewModel>>(usersMembers);
            viewModel.Screenshots = this.mapper.Map<IEnumerable<ScreenshotViewModel>>(screenshots);

            GameProjectRights? currentUserRights = await this.gameProjectAuthorizer.GetRightsForUserAsync(currentUserId, projectId);

            string currentUserBase64Id = Guids.ToUrlSafeBase64(currentUserId);
            foreach (GameProjectMemberViewModel member in viewModel.Members)
            {
                member.CanBeDeleted = member.Id != currentUserBase64Id
                    && currentUserRights!.ChangeMembers;
            }

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
            AppUser? user = await this.appUserRepository.GetFirstByEmailOrNullAsync(model.EmailOrUserName, ct)
                ?? await this.appUserRepository.GetFirstByUserNameOrNullAsync(model.EmailOrUserName, ct);
            if (user == null)
            {
                throw new BadRequestException("User does not exist");
            }
            GameProjectMember? existingMember = await this.gameProjectRepository.GetMemberAsync(id, user.Id, ct);
            if (existingMember != null)
            {
                throw new BadRequestException("User is already a project member");
            }

            await this.gameProjectRepository.AddMemberAsync(new GameProjectMember
            {
                AppUserId = user.Id,
                GameProjectId = id,
                MemberRole = MemberRole.Developer
            }, ct);

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

            IEnumerable<GameProjectMemberUser> allMembers = await gameProjectRepository.GetAllMembersAsync(projectId, ct);
            if (allMembers.Count() <= 1)
            {
                throw new BadRequestException("Cannot delete a single project member");
            }

            await this.gameProjectRepository.RemoveMemberAsync(projectId, userId, ct);

            return NoContent();
        }

        [HttpDelete("{gameProjectId}")]
        [Authorize(Filters.GameProjectAuthPolicy.ChangeMembers)]
        public async Task<IActionResult> DeleteProject([FromRoute] string gameProjectId)
        {
            Guid projectId = Guids.FromUrlSafeBase64(gameProjectId);
            await this.gameProjectDeleter.DeleteProjectAsync(projectId);
            return NoContent();
        }
    }
}
