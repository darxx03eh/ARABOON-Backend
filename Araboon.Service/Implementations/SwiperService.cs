using Araboon.Data.Entities;
using Araboon.Data.Response.Swipers.Queries;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Araboon.Service.Implementations
{
    public class SwiperService : ISwiperService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICloudinaryService cloudinaryService;
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<SwiperService> logger;

        public SwiperService(
            IUnitOfWork unitOfWork,
            ICloudinaryService cloudinaryService,
            AraboonDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SwiperService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.cloudinaryService = cloudinaryService;
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        public async Task<string> ActivateSwiperToggleAsync(int id)
        {
            logger.LogInformation("Attempt to toggle Swiper activation - محاولة تفعيل/تعطيل السلايدر | SwiperId: {Id}", id);

            var swiper = await unitOfWork.SwiperRepository.GetByIdAsync(id);
            if (swiper is null)
            {
                logger.LogWarning("Swiper not found - السلايدر غير موجود | SwiperId: {Id}", id);
                return "SwiperToActivateToggleNotFound";
            }

            try
            {
                if (!swiper.IsActive)
                {
                    if (swiper.Link.StartsWith("https://araboon.vercel.app/manga", StringComparison.OrdinalIgnoreCase))
                    {
                        var urlParts = swiper.Link.Split('/');
                        var index = Array.FindIndex(urlParts, x => x.Equals("manga", StringComparison.OrdinalIgnoreCase));
                        var mangaId = urlParts[index + 1];

                        var manga = await unitOfWork.MangaRepository.GetByIdAsync(Convert.ToInt32(mangaId));
                        if (manga is null)
                        {
                            logger.LogWarning("Cannot activate slider because linked manga not found - المانجا المرتبطة غير موجودة | MangaId: {Mid}", mangaId);
                            return "MangaNotFound";
                        }

                        if (!manga.IsActive)
                        {
                            logger.LogWarning("Cannot activate slider because linked manga is inactive - المانجا المرتبطة غير مفعلة | MangaId: {Mid}", mangaId);
                            return "CanNotActivateThisSwiperBecauseItIsLinkedToAnInactiveManga";
                        }
                    }

                    swiper.IsActive = true;
                }
                else
                {
                    swiper.IsActive = false;
                }

                await unitOfWork.SwiperRepository.UpdateAsync(swiper);

                logger.LogInformation("Swiper activation updated successfully - تم تعديل حالة السلايدر بنجاح | Active: {Active}", swiper.IsActive);

                return swiper.IsActive ? "ActivateSwiperSuccessfully" : "DeActivateSwiperSuccessfully";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error toggling swiper activation - خطأ أثناء عملية تفعيل/تعطيل السلايدر");
                return "AnErrorOccurredWhileActivatingOrDeActivatingProcess";
            }
        }

        public async Task<(string, Swiper?)> AddNewSwiperAsync(IFormFile image, string link, string? noteEn = null, string? noteAr = null)
        {
            logger.LogInformation("Adding new swiper - إضافة سلايدر جديد");

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var swiper = await unitOfWork.SwiperRepository.AddAsync(new Swiper()
                {
                    NoteEn = noteEn,
                    NoteAr = noteAr,
                    Link = link
                });

                if (swiper is null)
                {
                    logger.LogError("Failed to create swiper record - فشل إنشاء السلايدر في قاعدة البيانات");
                    return ("AnErrorOccurredWhileAddingSwiperProcess", null);
                }

                if (link.StartsWith("https://araboon.vercel.app/manga", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = link.Split('/');
                    var index = Array.FindIndex(parts, el => el.Equals("manga", StringComparison.OrdinalIgnoreCase));
                    var mangaId = parts[index + 1];

                    var manga = await unitOfWork.MangaRepository.GetByIdAsync(Convert.ToInt32(mangaId));
                    if (manga is null)
                    {
                        logger.LogWarning("Linked manga does not exist - المانجا المرتبطة غير موجودة");
                        await transaction.RollbackAsync();
                        return ("YouCanNotAddTheSwiperBecauseMangaNotExist", null);
                    }
                }

                if (image != null)
                {
                    var guidPart = Guid.NewGuid().ToString("N")[..12];
                    var datePart = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                    var fileId = $"{guidPart}-{datePart}";

                    using var stream = image.OpenReadStream();
                    var folder = $"ARABOON/Swipers/{swiper.SwiperId}/img";

                    var url = await cloudinaryService.UploadFileAsync(stream, folder, fileId);

                    swiper.ImageUrl = url;
                    swiper.UpdatedAt = DateTime.UtcNow;
                    await unitOfWork.SwiperRepository.UpdateAsync(swiper);
                }

                await transaction.CommitAsync();

                logger.LogInformation("Swiper added successfully - تم إضافة السلايدر بنجاح | SwiperId: {Id}", swiper.SwiperId);

                return ("SwiperAddedSuccessfully", swiper);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding new swiper - خطأ أثناء إضافة السلايدر");

                if (transaction.GetDbTransaction().Connection != null)
                    await transaction.RollbackAsync();

                return ("AnErrorOccurredWhileAddingSwiperProcess", null);
            }
        }

        public async Task<string> DeleteExistingSwiperAsync(int id)
        {
            logger.LogInformation("Deleting swiper - حذف السلايدر | SwiperId: {Id}", id);

            var swiper = await unitOfWork.SwiperRepository.GetByIdAsync(id);

            if (swiper is null)
            {
                logger.LogWarning("Swiper not found - السلايدر غير موجود");
                return "SwiperNotFound";
            }

            try
            {
                await unitOfWork.SwiperRepository.DeleteAsync(swiper);
                logger.LogInformation("Swiper deleted successfully - تم حذف السلايدر بنجاح");
                return "SwiperDeletedSuccessfully";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting swiper - خطأ أثناء حذف السلايدر");
                return "AnErrorOccurredWhileDeleteingSwiperProcess";
            }
        }

        public async Task<(string, IList<Swiper>?, SwiperMetaDataResponse?)> GetSwiperForDashboardAsync()
        {
            logger.LogInformation("Fetching all swipers for dashboard - جلب جميع السلايدرز للوحة التحكم");

            var swipers = await unitOfWork.SwiperRepository.GetTableNoTracking().ToListAsync();

            if (!swipers.Any())
            {
                logger.LogWarning("No swipers found - لا يوجد أي سلايدرز");
                return ("SwipersNotFound", null, null);
            }

            var meta = new SwiperMetaDataResponse()
            {
                TotalSwipers = swipers.Count,
                ActiveSwipers = swipers.Count(x => x.IsActive),
                InActiveSwipers = swipers.Count(x => !x.IsActive)
            };

            logger.LogInformation("Swipers fetched successfully - تم جلب السلايدرز بنجاح");

            return ("SwipersFound", swipers, meta);
        }

        public async Task<(string, IList<Swiper>?)> GetSwiperForHomePageAsync()
        {
            logger.LogInformation("Fetching active swipers for homepage - جلب السلايدرز المفعّلين للصفحة الرئيسية");

            var swipers = await unitOfWork.SwiperRepository
                .GetTableNoTracking()
                .Where(x => x.IsActive)
                .ToListAsync();

            if (!swipers.Any())
            {
                logger.LogWarning("No active swipers found - لا يوجد سلايدرز مفعّلين");
                return ("SwipersNotFound", null);
            }

            logger.LogInformation("Active swipers fetched successfully - تم جلب السلايدرز المفعّلين بنجاح");

            return ("SwipersFound", swipers);
        }

        public async Task<(string, Swiper?)> UpdateSwiperNoteLinkAsync(int id, string noteEn, string noteAr, string link)
        {
            logger.LogInformation("Updating swiper info - تعديل بيانات السلايدر | SwiperId: {Id}", id);

            var swiper = await unitOfWork.SwiperRepository.GetByIdAsync(id);

            if (swiper is null)
            {
                logger.LogWarning("Swiper not found - السلايدر غير موجود");
                return ("SwiperNotFound", null);
            }

            try
            {
                swiper.NoteEn = noteEn;
                swiper.NoteAr = noteAr;

                if (link.StartsWith("https://araboon.vercel.app/manga", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = link.Split('/');
                    var index = Array.FindIndex(parts, e => e.Equals("manga", StringComparison.OrdinalIgnoreCase));
                    var mangaId = parts[index + 1];

                    var manga = await unitOfWork.MangaRepository.GetByIdAsync(Convert.ToInt32(mangaId));

                    if (manga is null)
                        return ("MangaNotFound", null);

                    if (!manga.IsActive)
                        swiper.IsActive = false;
                }

                swiper.Link = link;
                swiper.UpdatedAt = DateTime.UtcNow;

                await unitOfWork.SwiperRepository.UpdateAsync(swiper);

                logger.LogInformation("Swiper updated successfully - تم تعديل السلايدر بنجاح");

                return ("SwiperUpdatedSuccessfully", swiper);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating swiper - خطأ أثناء تعديل السلايدر");
                return ("AnErrorOccurredWhileUpdatingSwiperNote", null);
            }
        }

        public async Task<(string, string?)> UploadNewSwiperImageAsync(int id, IFormFile image)
        {
            logger.LogInformation("Updating swiper image - تعديل صورة السلايدر | SwiperId: {Id}", id);

            var swiper = await unitOfWork.SwiperRepository.GetByIdAsync(id);

            if (swiper is null)
            {
                logger.LogWarning("Swiper not found - السلايدر غير موجود");
                return ("SwiperNotFound", null);
            }

            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                if (!string.IsNullOrWhiteSpace(swiper.ImageUrl))
                {
                    var deleted = await cloudinaryService.DeleteFileAsync(swiper.ImageUrl);

                    if (deleted == "FailedToDeleteImageFromCloudinary")
                    {
                        logger.LogError("Failed to delete old image - فشل حذف الصورة القديمة");
                        return ("FailedToDeleteOldImageFromCloudinary", null);
                    }
                }

                var guidPart = Guid.NewGuid().ToString("N")[..12];
                var time = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var fileId = $"{guidPart}-{time}";

                using var stream = image.OpenReadStream();
                var folder = $"ARABOON/Swipers/{swiper.SwiperId}/img";

                var url = await cloudinaryService.UploadFileAsync(stream, folder, fileId);
                swiper.ImageUrl = url;

                await unitOfWork.SwiperRepository.UpdateAsync(swiper);
                await transaction.CommitAsync();

                logger.LogInformation("Swiper image updated successfully - تم تعديل صورة السلايدر بنجاح");

                return ("TheImageHasBeenChangedSuccessfully", url);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating swiper image - خطأ أثناء تعديل صورة السلايدر");

                if (transaction.GetDbTransaction().Connection != null)
                    await transaction.RollbackAsync();

                return ("AnErrorOccurredWhileProcessingImageModificationRequest", null);
            }
        }
    }
}