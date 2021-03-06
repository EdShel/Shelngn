using AutoMapper;
using Shelngn.Entities;

namespace Shelngn.Api.GameProjects
{
    public class GameProjectMemberViewModel
    {
        public string Id { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string? AvatarUrl { get; set; }

        public string MemberRole { get; set; } = null!;

        public bool CanBeDeleted { get; set; }

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<GameProjectMemberUser, GameProjectMemberViewModel>()
                    .ForMember(m => m.Id, opt => opt.MapFrom(g => g.Id.ToUrlSafeBase64()));
            }
        }
    }

}
