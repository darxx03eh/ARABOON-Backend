using Araboon.Data.Entities;
using Araboon.Data.Response.Swipers.Queries;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Araboon.Service.Implementations
{
    public class SwiperService : ISwiperService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICloudinaryService cloudinaryService;
        private readonly AraboonDbContext context;

        public SwiperService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService, AraboonDbContext context)
        {
            this.unitOfWork = unitOfWork;
            this.cloudinaryService = cloudinaryService;
            this.context = context;
        }

        public async Task<string> ActivateSwiperToggleAsync(int id)
        {
            var swiper = await unitOfWork.SwiperRepository.GetByIdAsync(id);
            if (swiper is null)
                return "SwiperToActivateToggleNotFound";

            try
            {
                if (swiper.IsActive) swiper.IsActive = false;
                else swiper.IsActive = true;
                await unitOfWork.SwiperRepository.UpdateAsync(swiper);
                if (swiper.IsActive) return "ActivateSwiperSuccessfully";
                return "DeActivateSwiperSuccessfully";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileActivatingOrDeActivatingProcess";
            }
        }

        public async Task<(string, Swiper?)> AddNewSwiperAsync(IFormFile image, string link, string? note = null)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var swiper = await unitOfWork.SwiperRepository.AddAsync(new Swiper() { Note = note, Link = link});
                if (swiper is null)
                    return ("AnErrorOccurredWhileAddingSwiperProcess", null);

                if(image is not null)
                {
                    var guidPart = Guid.NewGuid().ToString("N").Substring(0, 12);
                    var datePart = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                    var id = $"{guidPart}-{datePart}";

                    using var stream = image.OpenReadStream();
                    var folderName = $"ARABOON/Swipers/{swiper.SwiperId}/img";
                    var url =  await cloudinaryService.UploadFileAsync(stream, folderName, id);
                    swiper.ImageUrl = url;
                    swiper.UpdatedAt = DateTime.UtcNow;
                    await unitOfWork.SwiperRepository.UpdateAsync(swiper);
                }
                await transaction.CommitAsync();
                return ("SwiperAddedSuccessfully", swiper);
            }catch(Exception exp)
            {
                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();
                return ("AnErrorOccurredWhileAddingSwiperProcess", null);
            }
        }

        public async Task<string> DeleteExistingSwiperAsync(int id)
        {
            var swiper = await unitOfWork.SwiperRepository.GetByIdAsync(id);
            if (swiper is null)
                return "SwiperNotFound";

            try
            {
                await unitOfWork.SwiperRepository.DeleteAsync(swiper);
                return "SwiperDeletedSuccessfully";
            }catch(Exception exp)
            {
                return "AnErrorOccurredWhileDeleteingSwiperProcess";
            }
        }

        public async Task<(string, IList<Swiper>?, SwiperMetaDataResponse?)> GetSwiperForDashboardAsync()
        {
            var swipers = await unitOfWork.SwiperRepository
                          .GetTableNoTracking()
                          .ToListAsync();

            if (!swipers.Any())
                return ("SwipersNotFound", null, null);

            return ("SwipersFound", swipers, new SwiperMetaDataResponse
            {
                TotalSwipers = swipers.Count(),
                ActiveSwipers = swipers.Where(swiper => swiper.IsActive).Count(),
                InActiveSwipers = swipers.Where(swiper => !swiper.IsActive).Count()
            });
        }

        public async Task<(string, IList<Swiper>?)> GetSwiperForHomePageAsync()
        {
            var swipers = await unitOfWork.SwiperRepository.GetTableNoTracking()
                          .Where(swiper => swiper.IsActive).ToListAsync();

            if (!swipers.Any())
                return ("SwipersNotFound", null);

            return ("SwipersFound", swipers);
        }

        public async Task<(string, Swiper?)> UpdateSwiperNoteLinkAsync(int id, string note, string link)
        {
            var swiper = await unitOfWork.SwiperRepository.GetByIdAsync(id);
            if (swiper is null)
                return ("SwiperNotFound", null);

            try
            {
                swiper.Note = note;
                swiper.UpdatedAt = DateTime.UtcNow;
                await unitOfWork.SwiperRepository.UpdateAsync(swiper);
                return ("SwiperNoteUpdatedSuccessfully", swiper);
            }
            catch(Exception exp)
            {
                return ("AnErrorOccurredWhileUpdatingSwiperNote", null);
            }
        }

        public async Task<(string, string?)> UploadNewSwiperImageAsync(int id, IFormFile image)
        {
            var swiper = await unitOfWork.SwiperRepository.GetByIdAsync(id);
            if (swiper is null)
                return ("SwiperNotFound", null);

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var originalUrl = swiper.ImageUrl;
                if (!string.IsNullOrWhiteSpace(originalUrl))
                {
                    var cloudinaryResult = await cloudinaryService.DeleteFileAsync(originalUrl);
                    if (cloudinaryResult.Equals("FailedToDeleteImageFromCloudinary"))
                        return ("FailedToDeleteOldImageFromCloudinary", null);
                }
                var guidPart = Guid.NewGuid().ToString("N").Substring(0, 12);
                var datePart = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var fullPart = $"{guidPart}-{datePart}";
                using (var stream = image.OpenReadStream())
                {
                    var (imageName, folderName) = (fullPart, $"ARABOON/Swipers/{swiper.SwiperId}/img");
                    var url = await cloudinaryService.UploadFileAsync(stream, folderName, imageName);
                    swiper.ImageUrl = url;
                }
                await unitOfWork.SwiperRepository.UpdateAsync(swiper);
                await transaction.CommitAsync();
                return ("TheImageHasBeenChangedSuccessfully", swiper.ImageUrl);
            }
            catch (Exception exp)
            {
                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();
                return ("AnErrorOccurredWhileProcessingImageModificationRequest", null);
            }
        }
    }
}
