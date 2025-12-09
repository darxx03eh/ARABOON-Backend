using AutoMapper;

namespace Araboon.Core.Mapping.Swipers
{
    public partial class SwiperProfile : Profile
    {
        public SwiperProfile()
        {
            GetSwiperForHomePageMapping();
            GetSwiperForDashboardMapping();
        }
    }
}
