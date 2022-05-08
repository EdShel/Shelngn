using AutoMapper;
using Shelngn.Entities;

namespace Shelngn.Api.Auth
{
    public partial class AuthController
    {
        public class CurrentUserModel
        {
            public string Id { get; set; } = null!;

            public string Email { get; set; } = null!;

            public string UserName { get; set; } = null!;
            public string AvatarUrl { get; set; } = null!;

            private class MappingProfile : Profile
            {
                public MappingProfile()
                {
                    CreateMap<AppUser, CurrentUserModel>()
                        .ForMember(m => m.Id, opt => opt.MapFrom(u => Guids.ToUrlSafeBase64(u.Id)));
                }
            }
        }
    }
}
