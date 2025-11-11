using Araboon.Data.Entities;
using Araboon.Data.Response.Swipers.Queries;

namespace Araboon.Core.Mapping.Swipers
{
    public partial class SwiperProfile
    {
        void GetSwiperForHomePageMapping()
        {
            CreateMap<Swiper, GetSwiperForHomePageResponse>()
                .ForMember(to => to.Id, from => from.MapFrom(src => src.SwiperId))
                .ForMember(to => to.Url, from => from.MapFrom(src => src.ImageUrl))
                .ForMember(to => to.Link, from => from.MapFrom(src => src.Link));
        }
    }
}
