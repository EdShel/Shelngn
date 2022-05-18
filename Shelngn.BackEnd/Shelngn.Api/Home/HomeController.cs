using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shelngn.Entities;
using Shelngn.Repositories;

namespace Shelngn.Api.Home
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IGameProjectRepository gameProjectRepository;
        private readonly IMapper mapper;

        public HomeController(IGameProjectRepository gameProjectRepository, IMapper mapper)
        {
            this.gameProjectRepository = gameProjectRepository;
            this.mapper = mapper;
        }

        [HttpGet("projects")]
        public async Task<IActionResult> GetRecentGameProjects(
            [FromQuery(Name = "until")] DateTimeOffset? optionalUntil,
            CancellationToken ct)
        {
            DateTimeOffset until = optionalUntil ?? DateTimeOffset.UtcNow;
            const int take = 10;
            var gameProjects = await gameProjectRepository.GetMostRecentGameProjectsAsync(until, take + 1, ct);
            var model = mapper.Map<IEnumerable<PublishedGameProjectModel>>(gameProjects.Take(take));
            bool hasMore = gameProjects.Count() > take;
            return Ok(new
            {
                Data = model,
                HasMore = hasMore,
                Until = !hasMore ? (DateTimeOffset?)null : gameProjects.Last().InsertDate
            });
        }
    }

    public class PublishedGameProjectModel
    {
        public string Id { get; set; } = null!;
        public string ProjectName { get; set; } = null!;
        public IList<ScreenshotModel> Screenshots { get; set; } = null!;
        public IList<UserModel> Members { get; set; } = null!;

        public class ScreenshotModel
        {
            public string Id { get; set; } = null!;
            public string ImageUrl { get; set; } = null!;
        }

        public class UserModel
        {
            public string Id { get; set; } = null!;
            public string UserName { get; set; } = null!;
        }

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<PublishedGameProjectScreenshot, ScreenshotModel>()
                    .ForMember(m => m.Id, opt => opt.MapFrom(s => Guids.ToUrlSafeBase64(s.Id)));
                CreateMap<PublishedGameProjectUserMember, UserModel>()
                    .ForMember(m => m.Id, opt => opt.MapFrom(s => Guids.ToUrlSafeBase64(s.Id)));
                CreateMap<PublishedGameProject, PublishedGameProjectModel>()
                    .ForMember(m => m.Id, opt => opt.MapFrom(s => Guids.ToUrlSafeBase64(s.Id)));
            }
        }
    }
}
