using Araboon.Data.Entities;
using Araboon.Data.Response.Swipers.Queries;
using Araboon.Infrastructure.Resolvers.SwipersResolver;

namespace Araboon.Core.Mapping.Swipers
{
    public partial class SwiperProfile
    {
        void GetSwiperForDashboardMapping()
        {
            CreateMap<Swiper, GetSwiperForDashboardResponse>()
                .ForMember(to => to.Id, from => from.MapFrom(src => src.SwiperId))
                .ForMember(to => to.Note, from => from.MapFrom(src => src.Note))
                .ForMember(to => to.Url, from => from.MapFrom(src => src.ImageUrl))
                .ForMember(to => to.IsActive, from => from.MapFrom(src => src.IsActive))
                .ForMember(to => to.CreatedAt, from => from.MapFrom<SwiperDateFormatResolver>());
        }
    }
}
