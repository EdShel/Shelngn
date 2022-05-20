using AutoMapper;
using Shelngn.Entities;

namespace Shelngn.Api.GameProjects
{
    public class PublicationInfoModel
    {
        public bool IsPublished { get; set; }
        public DateTimeOffset? Date { get; set; }

        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<GameProjectPublication?, PublicationInfoModel>()
                    .ConvertUsing(new TypeConverter());
            }

            private class TypeConverter : ITypeConverter<GameProjectPublication?, PublicationInfoModel>
            {
                public PublicationInfoModel Convert(GameProjectPublication? publ, PublicationInfoModel destination, ResolutionContext context)
                {
                    return publ == null
                        ? new PublicationInfoModel { IsPublished = false }
                        : new PublicationInfoModel
                        {
                            IsPublished = true,
                            Date = publ.InsertDate,
                        };
                }
            }
        }
    }
}
