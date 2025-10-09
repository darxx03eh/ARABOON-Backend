using Araboon.Data.Response.Ratings;

namespace Araboon.Service.Interfaces
{
    public interface IRatingService
    {
        public Task<(string, double?, int?)> RateAsync(int mangaId, double rate);
        public Task<string> DeleteRateAsync(int id);
        public Task<(string, GetRatingForManga?)> GetRatingsForMangaAsync(int mangaId);
    }
}
