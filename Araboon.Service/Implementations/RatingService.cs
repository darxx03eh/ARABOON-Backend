using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Ratings;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Service.Implementations
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AraboonUser> userManager;

        public RatingService(IUnitOfWork unitOfWork, UserManager<AraboonUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        public async Task<(string, double?)> DeleteRateAsync(int id)
        {
            var userId = unitOfWork.RatingsRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return ("UserNotFound", null);

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return ("UserNotFound", null);

            var rate = await unitOfWork.RatingsRepository.GetByIdAsync(id);
            if (rate is null)
                return ("RateNotFound", null);
            if (!rate.UserID.Equals(user.Id))
                return ("ThisRateDoNotBelongToYou", null);

            try
            {
                await unitOfWork.RatingsRepository.DeleteAsync(rate);
                var manga = await unitOfWork.MangaRepository.GetByIdAsync(rate.MangaID);
                if (manga.RatingsCount.Equals(1))
                    manga.Rate = 0;
                else var (result, totalStars) = await UpdateTotalStarsAsync(manga.MangaID);

                manga.RatingsCount--;
                await unitOfWork.MangaRepository.UpdateAsync(manga);
                return ("TheRateHasBeenSuccessfullyDeleted", manga.Rate);
            }
            catch (Exception exp)
            {
                return ("AnErrorOccurredWhileDeletingTheRate", null);
            }
        }

        public async Task<(string, GetRatingForManga?)> GetRatingsForMangaAsync(int mangaId)
        {
            var userId = unitOfWork.RatingsRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return ("UserNotFound", null);

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return ("UserNotFound", null);

            var rate = await unitOfWork.RatingsRepository.GetRateByMangaIdAndUserIdAsync(user.Id, mangaId);
            if (rate is null)
                return ("RateNotFound", null);

            return ("RateFound", new GetRatingForManga()
            {
                Id = rate.Id,
                Rate = rate.Rate,
            });
        }

        public async Task<(string, double?, int?, double?)> RateAsync(int mangaId, double rate)
        {
            var userId = unitOfWork.RatingsRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return ("UserNotFound", null, null, null);

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return ("UserNotFound", null, null, null);

            var manga = await unitOfWork.MangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return ("MangaNotFound", null, null, null);

            double newRate = 0.0;
            var isRateBefore = unitOfWork.RatingsRepository.IsUserMakeRateForMangaAsync(user.Id, mangaId);
            if (!isRateBefore)
            {
                var rateObject = await unitOfWork.RatingsRepository.AddAsync(new Ratings()
                {
                    UserID = user.Id,
                    MangaID = mangaId,
                    Rate = rate,
                });
                if (rateObject is null)
                    return ("AnErrorOccurredWhileAddingTheRate", null, null, null);
                var (result, totalStars) = await UpdateTotalStarsAsync(mangaId);
                manga.RatingsCount++;
                await unitOfWork.MangaRepository.UpdateAsync(manga);
                return ("TheRateHasBeenAddedSuccessfully", rate, rateObject.Id, totalStars);
            }
            else
            {
                var rateObject = await unitOfWork.RatingsRepository.GetRateByMangaIdAndUserIdAsync(user.Id, mangaId);
                rateObject.Rate = rate;
                try
                {
                    await unitOfWork.RatingsRepository.UpdateAsync(rateObject);
                    var (result, totalStars) = await UpdateTotalStarsAsync(mangaId);
                    await unitOfWork.MangaRepository.UpdateAsync(manga);
                    return ("TheRateHasBeenModifiedSuccessfully", rate, rateObject.Id, totalStars);
                }
                catch (Exception exp)
                {
                    return ("AnErrorOccurredWhileModifyingTheRate", null, null, null);
                }
            }
        }
        private async Task<(string, double?)> UpdateTotalStarsAsync(int mangaId)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return ("MangaNotFound", null);

            var ratings = await unitOfWork.RatingsRepository.GetTableNoTracking()
                            .Where(rate => rate.MangaID.Equals(mangaId)).ToListAsync();

            if (ratings.Any())
            {
                try
                {
                    manga.Rate = ratings.Average(rate => rate.Rate);
                    await unitOfWork.MangaRepository.UpdateAsync(manga);
                    return ("TotalStarsHaveBeenUpdatedSuccessfully", manga.Rate);
                }
                catch(Exception exp)
                {
                    return ("AnErrorOccurredWhileUpdatingTheTotalStars", null);
                }
            }
            return ("NoRateForThisMangaFound", null);
        }
    }
}
