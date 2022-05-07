using AutoMapper;
using Shelngn.Entities;

namespace Shelngn.Api.GameProjects
{
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
