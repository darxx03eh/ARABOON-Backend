using Araboon.Data.Entities;
using Araboon.Data.Response.Swipers.Queries;
using AutoMapper;

namespace Araboon.Infrastructure.Resolvers.SwipersResolver
{
    public class SwiperDateFormatResolver : IValueResolver<Swiper, GetSwiperForDashboardResponse, string>
    {
        public string Resolve(Swiper source, GetSwiperForDashboardResponse destination, string destMember, ResolutionContext context)
            => source.CreatedAt.HasValue ? source.CreatedAt.Value.ToString("yyyy-MM-dd") : "N/A";
    }
}
