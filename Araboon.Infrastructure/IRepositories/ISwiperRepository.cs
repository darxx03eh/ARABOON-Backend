using Araboon.Data.Entities;

namespace Araboon.Infrastructure.IRepositories
{
    public interface ISwiperRepository : IGenericRepository<Swiper>
    {
        public Task<bool> IsLinkExistsAsync(string link, int? excludeSwiperId = null);
    }
}
