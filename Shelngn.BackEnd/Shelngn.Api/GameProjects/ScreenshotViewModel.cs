using AutoMapper;
using Shelngn.Entities;

namespace Shelngn.Api.GameProjects
{
    public class ScreenshotViewModel
    {
        public string Id { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<GameProjectScreenshot, ScreenshotViewModel>()
                    .ForMember(m => m.Id, opt => opt.MapFrom(s => Guids.ToUrlSafeBase64(s.Id)));
            }
        }
    }
}
