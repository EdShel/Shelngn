using AutoMapper;
using Shelngn.Entities;

namespace Shelngn.Api.GameProjects
{
    public class GameProjectViewModel
    {
        public string Id { get; set; } = null!;
        public string ProjectName { get; set; } = null!;
        public DateTimeOffset InsertDate { get; set; }
        public bool IsPublished { get; set; }
        public IEnumerable<GameProjectMemberViewModel> Members { get; set; } = null!;

        private class MappongProfile : Profile
        {
            public MappongProfile()
            {
                CreateMap<GameProject, GameProjectViewModel>()
                    .ForMember(m => m.Id, opt => opt.MapFrom(g => g.Id.ToUrlSafeBase64()));
            }
        }
    }

}
