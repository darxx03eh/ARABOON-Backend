using Araboon.Data.Entities;

namespace Araboon.Infrastructure.IRepositories
{
    public interface IRatingsRepository : IGenericRepository<Ratings>
    {
        public bool IsUserMakeRateForMangaAsync(int userId, int mangaId);
        public Task<Ratings> GetRateByMangaIdAndUserIdAsync(int userId, int mangaId);
    }
}
