using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Ratings;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Araboon.Service.Implementations
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AraboonUser> userManager;
        private readonly ILogger<RatingService> logger;

        public RatingService(IUnitOfWork unitOfWork, UserManager<AraboonUser> userManager, ILogger<RatingService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<(string, double?)> DeleteRateAsync(int id)
        {
            logger.LogInformation("Deleting user rate - حذف تقييم المستخدم | RateId: {Id}", id);

            var userId = unitOfWork.RatingsRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogWarning("User not found - المستخدم غير موجود");
                return ("UserNotFound", null);
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User not found in DB - المستخدم غير موجود في قاعدة البيانات | UserId: {UserId}", userId);
                return ("UserNotFound", null);
            }

            var rate = await unitOfWork.RatingsRepository.GetByIdAsync(id);
            if (rate is null)
            {
                logger.LogWarning("Rate not found - التقييم غير موجود | RateId: {Id}", id);
                return ("RateNotFound", null);
            }

            if (!rate.UserID.Equals(user.Id))
            {
                logger.LogWarning("Rate does not belong to user - التقييم لا ينتمي للمستخدم | UserId: {UserId}", user.Id);
                return ("ThisRateDoNotBelongToYou", null);
            }

            try
            {
                await unitOfWork.RatingsRepository.DeleteAsync(rate);

                logger.LogInformation("Rate deleted successfully - تم حذف التقييم بنجاح | RateId: {Id}", id);

                var manga = await unitOfWork.MangaRepository.GetByIdAsync(rate.MangaID);

                if (manga.RatingsCount.Equals(1))
                {
                    manga.Rate = 0;
                }
                else
                {
                    var (result, totalStars) = await UpdateTotalStarsAsync(manga.MangaID);
                }

                manga.RatingsCount--;
                await unitOfWork.MangaRepository.UpdateAsync(manga);

                return ("TheRateHasBeenSuccessfullyDeleted", manga.Rate);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Error deleting rate - خطأ أثناء حذف التقييم | RateId: {Id}", id);
                return ("AnErrorOccurredWhileDeletingTheRate", null);
            }
        }

        public async Task<(string, GetRatingForManga?)> GetRatingsForMangaAsync(int mangaId)
        {
            logger.LogInformation("Getting rating for manga - جلب تقييم المانجا | MangaId: {MangaId}", mangaId);

            var userId = unitOfWork.RatingsRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogWarning("User not found in token - لم يتم العثور على المستخدم في التوكن");
                return ("UserNotFound", null);
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User not found in DB - المستخدم غير موجود | UserId: {UserId}", userId);
                return ("UserNotFound", null);
            }

            var rate = await unitOfWork.RatingsRepository.GetRateByMangaIdAndUserIdAsync(user.Id, mangaId);
            if (rate is null)
            {
                logger.LogWarning("Rate not found - لم يتم العثور على تقييم | MangaId: {MangaId}", mangaId);
                return ("RateNotFound", null);
            }

            logger.LogInformation("Rate found - تم العثور على تقييم | RateId: {RateId}", rate.Id);

            return ("RateFound", new GetRatingForManga()
            {
                Id = rate.Id,
                Rate = rate.Rate,
            });
        }

        public async Task<(string, double?, int?, double?)> RateAsync(int mangaId, double rate)
        {
            logger.LogInformation("Adding or updating rating - إضافة أو تعديل التقييم | MangaId: {MangaId}", mangaId);

            var userId = unitOfWork.RatingsRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
            {
                logger.LogWarning("User not found - المستخدم غير موجود");
                return ("UserNotFound", null, null, null);
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User not exist - المستخدم غير موجود | UserId: {UserId}", userId);
                return ("UserNotFound", null, null, null);
            }

            var manga = await unitOfWork.MangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - المانجا غير موجودة | MangaId: {MangaId}", mangaId);
                return ("MangaNotFound", null, null, null);
            }

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
                {
                    logger.LogError("Error adding rate - خطأ في إضافة التقييم | MangaId: {MangaId}", mangaId);
                    return ("AnErrorOccurredWhileAddingTheRate", null, null, null);
                }

                var (result, totalStars) = await UpdateTotalStarsAsync(mangaId);
                manga.RatingsCount++;
                await unitOfWork.MangaRepository.UpdateAsync(manga);

                logger.LogInformation("Rate added - تم إضافة التقييم | RateId: {RateId}", rateObject.Id);

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

                    logger.LogInformation("Rate updated - تم تعديل التقييم | RateId: {RateId}", rateObject.Id);

                    return ("TheRateHasBeenModifiedSuccessfully", rate, rateObject.Id, totalStars);
                }
                catch (Exception exp)
                {
                    logger.LogError(exp, "Error modifying rate - خطأ أثناء تعديل التقييم | RateId: {RateId}", rateObject.Id);
                    return ("AnErrorOccurredWhileModifyingTheRate", null, null, null);
                }
            }
        }

        private async Task<(string, double?)> UpdateTotalStarsAsync(int mangaId)
        {
            logger.LogInformation("Updating manga total stars - تحديث معدل تقييم المانجا | MangaId: {MangaId}", mangaId);

            var manga = await unitOfWork.MangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found while updating stars | MangaId: {MangaId}", mangaId);
                return ("MangaNotFound", null);
            }

            var ratings = await unitOfWork.RatingsRepository.GetTableNoTracking()
                .Where(rate => rate.MangaID.Equals(mangaId)).ToListAsync();

            if (ratings.Any())
            {
                try
                {
                    manga.Rate = ratings.Average(rate => rate.Rate);
                    await unitOfWork.MangaRepository.UpdateAsync(manga);

                    logger.LogInformation("Stars updated successfully - تم تحديث التقييمات بنجاح | NewRate: {Rate}", manga.Rate);

                    return ("TotalStarsHaveBeenUpdatedSuccessfully", manga.Rate);
                }
                catch (Exception exp)
                {
                    logger.LogError(exp, "Error updating total stars - خطأ أثناء تحديث معدل التقييم");
                    return ("AnErrorOccurredWhileUpdatingTheTotalStars", null);
                }
            }

            logger.LogWarning("No ratings found for manga - لا يوجد تقييمات للمانجا | MangaId: {MangaId}", mangaId);
            return ("NoRateForThisMangaFound", null);
        }
    }
}