using Araboon.Data.DTOs.Chapters;
using Araboon.Data.Entities;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Araboon.Service.Implementations
{
    public class ChapterService : IChapterService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AraboonDbContext context;
        private readonly ICloudinaryService cloudinaryService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<ChapterService> logger;

        public ChapterService(
            IUnitOfWork unitOfWork,
            AraboonDbContext context,
            ICloudinaryService cloudinaryService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ChapterService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.context = context;
            this.cloudinaryService = cloudinaryService;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }

        private async Task<IList<string>> TemporarilyStoreImagesAsync(IList<IFormFile> Images)
        {
            logger.LogInformation("Temporarily storing chapter images - تخزين صور الفصل مؤقتًا | Count: {Count}", Images?.Count);

            var tempPaths = new ConcurrentBag<string>();

            await Task.WhenAll(Images.Select(async file =>
            {
                var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
                await using var stream = System.IO.File.Create(tempPath);
                await file.CopyToAsync(stream);
                tempPaths.Add(tempPath);
            }));

            logger.LogInformation("Temporary images stored successfully - تم تخزين الصور المؤقتة | PathsCount: {Count}", tempPaths.Count);

            return tempPaths.ToList();
        }

        public async Task<(string, Chapter?, bool?, bool?)> AddNewChapterAsync(ChapterInfoDTO chapterInfo)
        {
            logger.LogInformation("Adding new chapter - إضافة فصل جديد | MangaId: {MangaId}, No: {No}", chapterInfo.MangaId, chapterInfo.ChapterNo);

            var manga = await unitOfWork.MangaRepository.GetByIdAsync(chapterInfo.MangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - المانجا غير موجودة | MangaId: {MangaId}", chapterInfo.MangaId);
                return ("MangaNotFound", null, null, null);
            }

            var imageUrl = "";
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                if (chapterInfo.Image is not null)
                {
                    logger.LogInformation("Uploading chapter cover image - رفع صورة غلاف الفصل | MangaId: {MangaId}, No: {No}", chapterInfo.MangaId, chapterInfo.ChapterNo);

                    imageUrl = await UploadChapterImageAsync(chapterInfo.Image, chapterInfo.MangaId, chapterInfo.ChapterNo, chapterInfo.Language);

                    if (imageUrl is null)
                    {
                        logger.LogError("Error uploading chapter cover image - خطأ أثناء رفع صورة غلاف الفصل");
                        return ("AnErrorOccurredWhileAddingTheImageForChapter", null, null, null);
                    }
                }

                var chapter = new Chapter()
                {
                    MangaID = chapterInfo.MangaId,
                    ChapterNo = chapterInfo.ChapterNo,
                    ArabicChapterTitle = chapterInfo.ArabicChapterTitle,
                    EnglishChapterTitle = chapterInfo.EnglishChapterTitle,
                    Language = chapterInfo.Language,
                    ImageUrl = imageUrl
                };

                var result = await unitOfWork.ChapterRepository.AddAsync(chapter);

                if (result is null)
                {
                    logger.LogError("Failed to add chapter - فشل في إضافة الفصل");
                    await transaction.RollbackAsync();
                    return ("AnErrorOccurredWhileAddingTheChapter", null, null, null);
                }

                logger.LogInformation("Storing chapter images temporarily - تخزين الصور مؤقتًا للرفع | ChapterId: {Id}", result.ChapterID);

                var tempPathsList = await TemporarilyStoreImagesAsync(chapterInfo.ChapterImages);

                BackgroundJob.Enqueue<IChapterService>(service =>
                    service.UploadChapterImagesAsync(
                        chapterInfo.MangaId,
                        chapterInfo.ChapterNo,
                        tempPathsList,
                        chapterInfo.Language,
                        result.ChapterID
                    ));

                logger.LogInformation("Queuing email notifications - جدولة إرسال إشعارات | MangaId: {MangaId}", chapterInfo.MangaId);

                var data = await unitOfWork.NotificationsRepository.GetEmailsToNewChapterNotify(chapterInfo.MangaId);
                var httpRequest = httpContextAccessor.HttpContext.Request;
                var domain = $"{httpRequest.Scheme}://{httpRequest.Host}";

                BackgroundJob.Enqueue<INotificationsService>(service =>
                    service.SendNotificationsAsync(
                        result.Manga.MangaNameEn,
                        chapterInfo.ChapterNo,
                        chapterInfo.EnglishChapterTitle,
                        chapterInfo.Language,
                        chapterInfo.Language.Equals("arabic", StringComparison.OrdinalIgnoreCase)
                            ? $"{domain}/manga/{chapterInfo.MangaId}/chapter/{chapterInfo.ChapterNo}?lang=ar"
                            : $"{domain}/manga/{chapterInfo.MangaId}/chapter/{chapterInfo.ChapterNo}?lang=en",
                        data
                    ));

                logger.LogInformation("Checking language continuity - التحقق من استمرارية الفصول | MangaId: {MangaId}", chapterInfo.MangaId);

                string lang = chapterInfo.Language.ToLower() == "arabic" ? "arabic" : "english";
                var chaptersForLang = await context.Chapters
                    .Where(c => c.MangaID == chapterInfo.MangaId && c.Language.ToLower().Equals(lang))
                    .OrderBy(c => c.ChapterNo)
                    .Select(c => c.ChapterNo)
                    .ToListAsync();

                bool noGapsAndStartsAtOne =
                    chaptersForLang.Count > 0 &&
                    chaptersForLang.First() == 1 &&
                    chaptersForLang.Zip(chaptersForLang.Skip(1), (a, b) => b - a).All(x => x == 1);

                string inactive = "";

                if (chapterInfo.Language.Equals("arabic", StringComparison.OrdinalIgnoreCase))
                {
                    if (!noGapsAndStartsAtOne)
                    {
                        logger.LogWarning("Arabic chapters have gaps - توجد فجوات في الفصول العربية");
                        manga.ArabicAvailable = false;
                        inactive = "Arabic";
                    }
                }
                else
                {
                    if (!noGapsAndStartsAtOne)
                    {
                        logger.LogWarning("English chapters have gaps - توجد فجوات في الفصول الإنجليزية");
                        manga.EnglishAvilable = false;
                        inactive = "English";
                    }
                }

                await unitOfWork.MangaRepository.UpdateAsync(manga);
                await transaction.CommitAsync();

                if (inactive != "")
                {
                    logger.LogWarning("Chapter added but language deactivated - تم إضافة الفصل ولكن اللغة تعطلت");
                    return ($"ChapterAddedSuccessfullyAnd{inactive}BecameInactiveDueToIncompleteChapters", result, manga.ArabicAvailable, manga.EnglishAvilable);
                }

                logger.LogInformation("Chapter added successfully - تم إضافة الفصل بنجاح | ChapterId: {Id}", result.ChapterID);
                return ("ChapterAddedSuccessfully", result, manga.ArabicAvailable, manga.EnglishAvilable);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding chapter - خطأ أثناء إضافة الفصل");

                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();

                return ("AnErrorOccurredWhileAddingTheChapter", null, null, null);
            }
        }

        public async Task UploadChapterImagesAsync(int mangaId, int chapterNo, IList<string> imagePaths, string lang, int chapterId)
        {
            logger.LogInformation("Uploading chapter images - رفع صور الفصل في الخلفية | MangaId: {MangaId}, ChapterNo: {No}", mangaId, chapterNo);

            int order = 1;
            string language = lang.ToLower() == "arabic" ? "ar" : "en";

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                if (language == "ar")
                {
                    IList<ArabicChapterImages> list = new List<ArabicChapterImages>();

                    foreach (var path in imagePaths)
                    {
                        var (folder, id) = GenerateFolderNameAndId(mangaId, chapterNo, lang);

                        using var stream = System.IO.File.OpenRead(path);
                        var url = await cloudinaryService.UploadFileAsync(stream, folder, id);

                        list.Add(new ArabicChapterImages()
                        {
                            ChapterID = chapterId,
                            ImageUrl = url,
                            OrderImage = order++
                        });

                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                    }

                    await unitOfWork.ArabicChapterImagesRepository.AddRangeAsync(list);
                }
                else
                {
                    IList<EnglishChapterImages> list = new List<EnglishChapterImages>();

                    foreach (var path in imagePaths)
                    {
                        var (folder, id) = GenerateFolderNameAndId(mangaId, chapterNo, lang);

                        using var stream = System.IO.File.OpenRead(path);
                        var url = await cloudinaryService.UploadFileAsync(stream, folder, id);

                        list.Add(new EnglishChapterImages()
                        {
                            ChapterID = chapterId,
                            ImageUrl = url,
                            OrderImage = order++
                        });

                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                    }

                    await unitOfWork.EnglishChapterImagesRepository.AddRangeAsync(list);
                }

                await transaction.CommitAsync();

                logger.LogInformation("Chapter images uploaded successfully - تم رفع صور الفصل بنجاح");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading chapter images - خطأ أثناء رفع صور الفصل");
                await transaction.RollbackAsync();
                throw;
            }
        }

        private (string, string) GenerateFolderNameAndId(int mangaId, int chapterNo, string lang)
        {
            string language = lang.ToLower() == "arabic" ? "ar" : "en";

            var guid = Guid.NewGuid().ToString("N")[..12];
            var date = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var id = $"{guid}-{date}";
            var folder = $"ARABOON/Mangas/{mangaId}/Chapters/{language}/{chapterNo}";

            return (folder, id);
        }

        private async Task<string?> UploadChapterImageAsync(IFormFile image, int mangaId, int chapterNo, string lang)
        {
            logger.LogInformation("Uploading single chapter image - رفع صورة واحدة للفصل | MangaId: {MangaId}, No: {No}", mangaId, chapterNo);

            string language = lang.ToLower() == "arabic" ? "ar" : "en";

            try
            {
                var guid = Guid.NewGuid().ToString("N")[..12];
                var date = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var id = $"{guid}-{date}";

                using var stream = image.OpenReadStream();

                var folder = $"ARABOON/Mangas/{mangaId}/Chapters/{language}/{chapterNo}/img";
                var url = await cloudinaryService.UploadFileAsync(stream, folder, id);

                logger.LogInformation("Chapter cover uploaded successfully - تم رفع صورة الغلاف بنجاح");

                return url;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading chapter cover - خطأ أثناء رفع صورة الغلاف");
                return null;
            }
        }

        public async Task<(string, int?)> ChapterReadAsync(int chapterId)
        {
            logger.LogInformation("Increasing chapter views - زيادة عدد المشاهدات | ChapterId: {Id}", chapterId);

            var chapter = await unitOfWork.ChapterRepository.GetByIdAsync(chapterId);

            if (chapter is null)
            {
                logger.LogWarning("Chapter not found - لم يتم العثور على الفصل | ChapterId: {Id}", chapterId);
                return ("ChapterNotFound", null);
            }

            try
            {
                chapter.ReadersCount++;
                await unitOfWork.ChapterRepository.UpdateAsync(chapter);

                logger.LogInformation("View counter increased - تم زيادة عدد المشاهدات");

                return ("ViewsIncreasedBy1", chapter.ReadersCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error increasing chapter view - خطأ أثناء زيادة المشاهدات");
                return ("AnErrorOccurredWhileIncreasingTheViewByOne", null);
            }
        }

        public async Task<(string, IList<Chapter>?, bool?, bool?)> GetChaptersForSpecificMangaByLanguage(int mangaId, string language)
        {
            logger.LogInformation("Retrieving chapters by language - جلب الفصول حسب اللغة | MangaId: {Id}, Lang: {Lang}", mangaId, language);

            var (message, chapters, isAr, isEn) =
                await unitOfWork.ChapterRepository.GetChaptersForSpecificMangaByLanguage(mangaId, language);

            return message switch
            {
                "MangaNotFound" =>
                (
                    "MangaNotFound",
                    null, null, null
                ),

                "TheLanguageYouRequestedIsNotAvailableForThisManga" =>
                (
                    "TheLanguageYouRequestedIsNotAvailableForThisManga",
                    null, null, null
                ),

                "ThereAreNoChaptersYet" =>
                (
                    "ThereAreNoChaptersYet",
                    null, null, null
                ),

                "TheChaptersWereFound" =>
                (
                    "TheChaptersWereFound",
                    chapters, isAr, isEn
                ),

                _ =>
                (
                    "ThereAreNoChaptersYet",
                    null, null, null
                )
            };
        }

        public async Task<(string, bool?, bool?)> DeleteExistingChapterAsync(int id)
        {
            logger.LogInformation("Deleting chapter - حذف الفصل | ChapterId: {Id}", id);

            var chapter = await unitOfWork.ChapterRepository.GetByIdAsync(id);

            if (chapter is null)
            {
                logger.LogWarning("Chapter not found - الفصل غير موجود | ChapterId: {Id}", id);
                return ("ChapterNotFound", null, null);
            }

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                logger.LogInformation("Deleting chapter views - حذف مشاهدات الفصل");

                var views = await unitOfWork.ChapterViewRepository.GetTableNoTracking()
                    .Where(x => x.ChapterID == chapter.ChapterID)
                    .ToListAsync();

                await unitOfWork.ChapterViewRepository.DeleteRangeAsync(views);

                logger.LogInformation("Deleting chapter images from db - حذف صور الفصل من قاعدة البيانات");

                if (chapter.Language.ToLower() == "arabic")
                {
                    var imgs = await unitOfWork.ArabicChapterImagesRepository.GetTableAsTracking()
                        .Where(x => x.ChapterID == chapter.ChapterID)
                        .ToListAsync();

                    await unitOfWork.ArabicChapterImagesRepository.DeleteRangeAsync(imgs);
                }
                else
                {
                    var imgs = await unitOfWork.EnglishChapterImagesRepository.GetTableAsTracking()
                        .Where(x => x.ChapterID == chapter.ChapterID)
                        .ToListAsync();

                    await unitOfWork.EnglishChapterImagesRepository.DeleteRangeAsync(imgs);
                }

                logger.LogInformation("Deleting chapter record - حذف سجل الفصل");

                await unitOfWork.ChapterRepository.DeleteAsync(chapter);

                logger.LogInformation("Checking language availability after deletion - التحقق من تفعيل اللغة بعد الحذف");

                string lang = chapter.Language.ToLower() == "arabic" ? "arabic" : "english";
                var chaptersForLang = await context.Chapters
                    .Where(c => c.MangaID == chapter.MangaID && c.Language.ToLower() == lang)
                    .OrderBy(c => c.ChapterNo)
                    .Select(c => c.ChapterNo)
                    .ToListAsync();

                bool noGapsAndStartsAtOne = false;

                if (chaptersForLang.Count > 0)
                {
                    var ordered = chaptersForLang.OrderBy(x => x).ToList();
                    noGapsAndStartsAtOne =
                        ordered.First() == 1 &&
                        ordered.Zip(ordered.Skip(1), (a, b) => b - a).All(x => x == 1);
                }

                var manga = await unitOfWork.MangaRepository.GetByIdAsync(chapter.MangaID);
                string msg;

                if (manga != null)
                {
                    if (lang == "arabic")
                    {
                        if (!noGapsAndStartsAtOne)
                            manga.ArabicAvailable = false;

                        msg = noGapsAndStartsAtOne
                            ? "ChapterDeletedSuccessfully"
                            : "ChapterDeletedSuccessfullyAndArabicBecameInactiveDueToIncompleteChapters";
                    }
                    else
                    {
                        if (!noGapsAndStartsAtOne)
                            manga.EnglishAvilable = false;

                        msg = noGapsAndStartsAtOne
                            ? "ChapterDeletedSuccessfully"
                            : "ChapterDeletedSuccessfullyAndEnglishBecameInactiveDueToIncompleteChapters";
                    }

                    await unitOfWork.MangaRepository.UpdateAsync(manga);
                }
                else msg = "ChapterDeletedSuccessfully";

                logger.LogInformation("Deleting chapter images from cloud - حذف الصور من كلاوديناري");

                foreach (var img in chapter.ArabicChapterImages.Select(x => x.ImageUrl)
                         .Concat(chapter.EnglishChapterImages.Select(x => x.ImageUrl))
                         .Where(url => !string.IsNullOrWhiteSpace(url)))
                {
                    BackgroundJob.Enqueue<ICloudinaryService>(x => x.DeleteFileAsync(img));
                }

                if (!string.IsNullOrWhiteSpace(chapter.ImageUrl))
                    BackgroundJob.Enqueue<ICloudinaryService>(x => x.DeleteFileAsync(chapter.ImageUrl));

                await transaction.CommitAsync();

                logger.LogInformation("Chapter deleted successfully - تم حذف الفصل بنجاح");

                return (msg, manga?.ArabicAvailable, manga?.EnglishAvilable);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting chapter - خطأ أثناء حذف الفصل");

                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();

                return ("AnErrorOccurredWhileDeletingTheChapter", null, null);
            }
        }

        public async Task<(string, Chapter?, bool?, bool?)> UpdateExistingChapterAsync(int id, int chapterNo, string arabicChapterTitle, string englishChapterTitle, string language)
        {
            logger.LogInformation("Updating chapter - تعديل الفصل | ChapterId: {Id}", id);

            var chapter = await unitOfWork.ChapterRepository.GetByIdAsync(id);

            if (chapter is null)
            {
                logger.LogWarning("Chapter not found - لم يتم العثور على الفصل | ChapterId: {Id}", id);
                return ("ChapterNotFound", null, null, null);
            }

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                chapter.ChapterNo = chapterNo;
                chapter.ArabicChapterTitle = arabicChapterTitle;
                chapter.EnglishChapterTitle = englishChapterTitle;
                chapter.Language = language;

                await unitOfWork.ChapterRepository.UpdateAsync(chapter);

                logger.LogInformation("Checking chapter continuity - التحقق من استمرارية الفصول");

                string lang = language.ToLower() == "arabic" ? "arabic" : "english";

                var chaptersForLang = await context.Chapters
                    .Where(c => c.MangaID == chapter.MangaID && c.Language.ToLower() == lang)
                    .OrderBy(c => c.ChapterNo)
                    .Select(c => c.ChapterNo)
                    .ToListAsync();

                bool noGapsAndStartsAtOne = false;

                if (chaptersForLang.Count > 0)
                {
                    var ordered = chaptersForLang.OrderBy(x => x).ToList();
                    noGapsAndStartsAtOne =
                        ordered.First() == 1 &&
                        ordered.Zip(ordered.Skip(1), (a, b) => b - a).All(x => x == 1);
                }

                var manga = await unitOfWork.MangaRepository.GetByIdAsync(chapter.MangaID);

                if (manga != null)
                {
                    if (lang == "arabic")
                    {
                        if (!noGapsAndStartsAtOne)
                            manga.ArabicAvailable = false;
                    }
                    else
                    {
                        if (!noGapsAndStartsAtOne)
                            manga.EnglishAvilable = false;
                    }

                    await unitOfWork.MangaRepository.UpdateAsync(manga);
                }

                await transaction.CommitAsync();

                logger.LogInformation("Chapter updated successfully - تم تعديل الفصل بنجاح");

                return ("ChapterUpdatedSuccessfully", chapter, manga?.ArabicAvailable, manga?.EnglishAvilable);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating chapter - خطأ أثناء تعديل الفصل");

                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();

                return ("AnErrorOccurredWhileUpdatingTheChapter", null, null, null);
            }
        }

        public async Task<(string, string?)> UploadChapterImageAsync(int id, IFormFile image)
        {
            logger.LogInformation("Updating chapter main image - تعديل صورة الفصل الرئيسية | ChapterId: {Id}", id);

            var chapter = await unitOfWork.ChapterRepository.GetByIdAsync(id);

            if (chapter is null)
            {
                logger.LogWarning("Chapter not found - لم يتم العثور على الفصل | ChapterId: {Id}", id);
                return ("ChapterNotFound", null);
            }

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var original = chapter.ImageUrl;

                if (!string.IsNullOrWhiteSpace(original))
                {
                    logger.LogInformation("Deleting old image - حذف الصورة القديمة | Url: {Url}", original);

                    var res = await cloudinaryService.DeleteFileAsync(original);

                    if (res == "FailedToDeleteImageFromCloudinary")
                    {
                        logger.LogError("Cloudinary delete failed - فشل حذف الصورة القديمة");
                        return ("FailedToDeleteOldImageFromCloudinary", null);
                    }
                }

                var guid = Guid.NewGuid().ToString("N")[..12];
                var date = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var idStr = $"{guid}-{date}";

                using (var stream = image.OpenReadStream())
                {
                    var lang = chapter.Language.Equals("arabic", StringComparison.OrdinalIgnoreCase) ? "ar" : "en";

                    var folder = $"ARABOON/Mangas/{chapter.MangaID}/Chapters/{lang}/{chapter.ChapterNo}/img";

                    var url = await cloudinaryService.UploadFileAsync(stream, folder, idStr);

                    chapter.ImageUrl = url;
                }

                await unitOfWork.ChapterRepository.UpdateAsync(chapter);
                await transaction.CommitAsync();

                logger.LogInformation("Chapter image updated successfully - تم تعديل الصورة بنجاح");

                return ("TheImageHasBeenChangedSuccessfully", chapter.ImageUrl);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating chapter image - خطأ أثناء تعديل صورة الفصل");

                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();

                return ("AnErrorOccurredWhileProcessingImageModificationRequest", null);
            }
        }

        public async Task<string> UploadChapterImagesAsync(int id, IList<IFormFile> images)
        {
            logger.LogInformation("Replacing all chapter images - استبدال جميع صور الفصل | ChapterId: {Id}", id);

            var chapter = await unitOfWork.ChapterRepository.GetByIdAsync(id);

            if (chapter is null)
            {
                logger.LogWarning("Chapter not found - لم يتم العثور على الفصل | ChapterId: {Id}", id);
                return "ChapterNotFound";
            }

            IList<string> imageUrls = chapter.Language.ToLower() == "arabic"
                ? chapter.ArabicChapterImages.Select(x => x.ImageUrl).ToList()
                : chapter.EnglishChapterImages.Select(x => x.ImageUrl).ToList();

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                logger.LogInformation("Deleting old chapter images from DB - حذف الصور القديمة من قاعدة البيانات");

                if (chapter.Language.ToLower() == "arabic")
                {
                    var imgs = await unitOfWork.ArabicChapterImagesRepository.GetTableAsTracking()
                        .Where(x => x.ChapterID == chapter.ChapterID)
                        .ToListAsync();

                    await unitOfWork.ArabicChapterImagesRepository.DeleteRangeAsync(imgs);
                }
                else
                {
                    var imgs = await unitOfWork.EnglishChapterImagesRepository.GetTableAsTracking()
                        .Where(x => x.ChapterID == chapter.ChapterID)
                        .ToListAsync();

                    await unitOfWork.EnglishChapterImagesRepository.DeleteRangeAsync(imgs);
                }

                logger.LogInformation("Scheduling deletion of images from cloud - جدولة حذف الصور من كلاوديناري");

                foreach (var url in imageUrls.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    BackgroundJob.Enqueue<ICloudinaryService>(x => x.DeleteFileAsync(url));
                }

                logger.LogInformation("Temporarily storing new images - تخزين الصور الجديدة مؤقتًا");

                var tempPathsList = await TemporarilyStoreImagesAsync(images);

                BackgroundJob.Enqueue<IChapterService>(service =>
                    service.UploadChapterImagesAsync(
                        chapter.MangaID,
                        chapter.ChapterNo,
                        tempPathsList,
                        chapter.Language,
                        chapter.ChapterID
                    ));

                await transaction.CommitAsync();

                logger.LogInformation("New chapter images are uploading in background - يتم رفع الصور الجديدة في الخلفية");

                return "ImagesAreBeingUploadedToCloudStoragePleaseWaitALittleWhile";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error uploading chapter images - خطأ أثناء رفع صور الفصل");

                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();

                return "AnErrorOccurredWhileProcessingTheImagesIploadRequest";
            }
        }
    }
}