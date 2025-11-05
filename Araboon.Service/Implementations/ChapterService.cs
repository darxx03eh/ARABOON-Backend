using Araboon.Data.DTOs.Chapters;
using Araboon.Data.Entities;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Concurrent;

namespace Araboon.Service.Implementations
{
    public class ChapterService : IChapterService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AraboonDbContext context;
        private readonly ICloudinaryService cloudinaryService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ChapterService(IUnitOfWork unitOfWork, AraboonDbContext context, ICloudinaryService cloudinaryService, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.context = context;
            this.cloudinaryService = cloudinaryService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<(string, Chapter?)> AddNewChapterAsync(ChapterInfoDTO chapterInfo)
        {
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(chapterInfo.MangaId);
            if (manga is null)
                return ("MangaNotFound", null);

            var imageUrl = "";
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                if (chapterInfo.Image is not null)
                {
                    imageUrl = await UploadChapterImageAsync(
                        chapterInfo.Image, chapterInfo.MangaId, chapterInfo.ChapterNo, chapterInfo.Language
                    );
                    if (imageUrl is null)
                        return ("AnErrorOccurredWhileAddingTheImageForChapter", null);
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
                    await transaction.RollbackAsync();
                    return ("AnErrorOccurredWhileAddingTheChapter", null);
                }

                var tempPaths = new ConcurrentBag<string>();
                await Task.WhenAll(chapterInfo.ChapterImages.Select(async file =>
                {
                    var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
                    await using var stream = System.IO.File.Create(tempPath);
                    await file.CopyToAsync(stream);
                    tempPaths.Add(tempPath);
                }));
                var tempPathsList = tempPaths.ToList();
                BackgroundJob.Enqueue<IChapterService>(service => service.UploadChapterImagesAsync(
                    chapterInfo.MangaId, chapterInfo.ChapterNo, tempPathsList, chapterInfo.Language, result.ChapterID
                ));

                var data = await unitOfWork.NotificationsRepository.GetEmailsToNewChapterNotify(chapterInfo.MangaId);
                var httpRequest = httpContextAccessor.HttpContext.Request;
                var domain = $"{httpRequest.Scheme}://{httpRequest.Host}";
                BackgroundJob.Enqueue<INotificationsService>(service => service.SendNotificationsAsync(
                    result.Manga.MangaNameEn,
                    chapterInfo.ChapterNo,
                    chapterInfo.EnglishChapterTitle,
                    chapterInfo.Language,
                    chapterInfo.Language.Equals("arabic", StringComparison.OrdinalIgnoreCase)
                        ? $"{domain}/manga/{chapterInfo.MangaId}/chapter/{chapterInfo.ChapterNo}?lang=ar"
                        : $"{domain}/manga/{chapterInfo.MangaId}/chapter/{chapterInfo.ChapterNo}?lang=en",
                    data
                ));

                string lang = chapterInfo.Language.ToLower() == "arabic" ? "ar" : "en";
                var chaptersForLang = await context.Chapters
                    .Where(c => c.MangaID == chapterInfo.MangaId && c.Language.ToLower().Contains(lang))
                    .OrderBy(c => c.ChapterNo)
                    .Select(c => c.ChapterNo)
                    .ToListAsync();

                bool noGapsAndStartsAtOne = chaptersForLang.Count > 0 &&
                                            chaptersForLang.First() == 1 &&
                                            chaptersForLang.Zip(chaptersForLang.Skip(1), (a, b) => b - a).All(diff => diff == 1);

                string inactiveLang = string.Empty;

                if (chapterInfo.Language.Equals("arabic", StringComparison.OrdinalIgnoreCase))
                {
                    if (!noGapsAndStartsAtOne)
                    {
                        manga.ArabicAvailable = false;
                        inactiveLang = "Arabic";
                    }
                    else if (Convert.ToBoolean(manga.ArabicAvailable)) manga.ArabicAvailable = true;
                }
                else
                {
                    if (!noGapsAndStartsAtOne)
                    {
                        manga.EnglishAvilable = false;
                        inactiveLang = "English";
                    }
                    else if (Convert.ToBoolean(manga.EnglishAvilable)) manga.EnglishAvilable = true;
                }

                await unitOfWork.MangaRepository.UpdateAsync(manga);
                await transaction.CommitAsync();

                if (!string.IsNullOrEmpty(inactiveLang))
                    return ($"ChapterAddedSuccessfullyAnd{inactiveLang}BecameInactiveDueToIncompleteChapters", result);

                return ("ChapterAddedSuccessfully", result);
            }
            catch
            {
                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();
                return ("AnErrorOccurredWhileAddingTheChapter", null);
            }
        }


        public async Task UploadChapterImagesAsync(int mangaId, int chapterNo, IList<string> imagePaths, string lang, int chapterId)
        {
            int order = 1;
            string language = lang.ToLower() == "arabic" ? "ar" : "en";
            var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (language.Equals("ar"))
                {
                    IList<ArabicChapterImages> chapterImages = new List<ArabicChapterImages>();
                    foreach (var path in imagePaths)
                    {
                        var (folderName, id) = GenerateFolderNameAndId(mangaId, chapterNo, lang);
                        using var stream = System.IO.File.OpenRead(path);
                        var url = await cloudinaryService.UploadFileAsync(stream, folderName, id);

                        var chapter = new ArabicChapterImages()
                        {
                            ChapterID = chapterId,
                            ImageUrl = url,
                            OrderImage = order++
                        };
                        chapterImages.Add(chapter);

                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                    }
                    await unitOfWork.ArabicChapterImagesRepository.AddRangeAsync(chapterImages);
                }
                else
                {
                    IList<EnglishChapterImages> chapterImages = new List<EnglishChapterImages>();
                    foreach (var path in imagePaths)
                    {
                        var (folderName, id) = GenerateFolderNameAndId(mangaId, chapterNo, lang);
                        using var stream = System.IO.File.OpenRead(path);
                        var url = await cloudinaryService.UploadFileAsync(stream, folderName, id);

                        var chapter = new EnglishChapterImages()
                        {
                            ChapterID = chapterId,
                            ImageUrl = url,
                            OrderImage = order++
                        };
                        chapterImages.Add(chapter);

                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                    }
                    await unitOfWork.EnglishChapterImagesRepository.AddRangeAsync(chapterImages);
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        private (string, string) GenerateFolderNameAndId(int mangaId, int chapterNo, string lang)
        {
            string language = lang.ToLower().Equals("arabic") ? "ar" : "en";
            var guidPart = Guid.NewGuid().ToString("N").Substring(0, 12);
            var datePart = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var id = $"{guidPart}-{datePart}";
            var folderName = $"ARABOON/Mangas/{mangaId}/Chapters/{language}/{chapterNo}";
            return (folderName, id);
        }
        private async Task<string?> UploadChapterImageAsync(IFormFile image, int mangaId, int chapterNo, string lang)
        {
            string language = lang.ToLower().Equals("arabic") ? "ar" : "en";
            try
            {
                var guidPart = Guid.NewGuid().ToString("N").Substring(0, 12);
                var datePart = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var id = $"{guidPart}-{datePart}";

                using var stream = image.OpenReadStream();
                var folderName = $"ARABOON/Mangas/{mangaId}/Chapters/{language}/{chapterNo}/img";
                return await cloudinaryService.UploadFileAsync(stream, folderName, id);
            }
            catch
            {
                return null;
            }
        }
        public async Task<(string, int?)> ChapterReadAsync(int chapterId)
        {
            var chapter = await unitOfWork.ChapterRepository.GetByIdAsync(chapterId);
            if (chapter is null)
                return ("ChapterNotFound", null);

            try
            {
                chapter.ReadersCount++;
                await unitOfWork.ChapterRepository.UpdateAsync(chapter);
                return ("ViewsIncreasedBy1", chapter.ReadersCount);
            }catch(Exception exp)
            {
                return ("AnErrorOccurredWhileIncreasingTheViewByOne", null);
            }
        }

        public async Task<(string, IList<Chapter>?)> GetChaptersForSpecificMangaByLanguage(int mangaId, string language)
        {
            var (message, chapters) = await unitOfWork.ChapterRepository.GetChaptersForSpecificMangaByLanguage(mangaId, language);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "TheLanguageYouRequestedIsNotAvailableForThisManga" => ("TheLanguageYouRequestedIsNotAvailableForThisManga", null),
                "ThereAreNoChaptersYet" => ("ThereAreNoChaptersYet", null),
                "TheChaptersWereFound" => ("TheChaptersWereFound", chapters),
                _ => ("ThereAreNoChaptersYet", null)
            };
        }

        public async Task<string> DeleteExistingChapterAsync(int id)
        {
            var chapter = await unitOfWork.ChapterRepository.GetByIdAsync(id);
            if (chapter is null)
                return "ChapterNotFound";

            await using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                IList<string> imageUrls = chapter.Language.ToLower() == "arabic"
                    ? chapter.ArabicChapterImages.Select(i => i.ImageUrl).ToList()
                    : chapter.EnglishChapterImages.Select(i => i.ImageUrl).ToList();

                var mainImage = chapter.ImageUrl;

                await unitOfWork.ChapterRepository.DeleteAsync(chapter);

                string lang = chapter.Language.ToLower() == "arabic" ? "ar" : "en";
                var chaptersForLang = await context.Chapters
                    .Where(c => c.MangaID == chapter.MangaID && c.Language.ToLower().Contains(lang))
                    .OrderBy(c => c.ChapterNo)
                    .Select(c => c.ChapterNo)
                    .ToListAsync();

                bool noGapsAndStartsAtOne = false;
                if (chaptersForLang.Count > 0)
                {
                    var ordered = chaptersForLang.OrderBy(n => n).ToList();
                    noGapsAndStartsAtOne = ordered.First() == 1 &&
                                           ordered.Zip(ordered.Skip(1), (a, b) => b - a).All(diff => diff == 1);
                }

                var manga = await unitOfWork.MangaRepository.GetByIdAsync(chapter.MangaID);
                string responseMessage;
                if (manga != null)
                {
                    if (chapter.Language.Equals("arabic", StringComparison.OrdinalIgnoreCase))
                    {
                        manga.ArabicAvailable = noGapsAndStartsAtOne;
                        responseMessage = noGapsAndStartsAtOne
                            ? "ChapterDeletedSuccessfully"
                            : "ChapterDeletedSuccessfullyAndArabicBecameInactiveDueToIncompleteChapters";
                    }

                    else manga.EnglishAvilable = noGapsAndStartsAtOne;
                    responseMessage = noGapsAndStartsAtOne
                        ? "ChapterDeletedSuccessfully"
                        : "ChapterDeletedSuccessfullyAndEnglishBecameInactiveDueToIncompleteChapters";

                    await unitOfWork.MangaRepository.UpdateAsync(manga);
                }
                else responseMessage = "ChapterDeletedSuccessfully";

                foreach (var url in imageUrls)
                    if(!string.IsNullOrWhiteSpace(url))
                        BackgroundJob.Enqueue<ICloudinaryService>(service => service.DeleteFileAsync(url));
                if (!string.IsNullOrWhiteSpace(mainImage))
                    BackgroundJob.Enqueue<ICloudinaryService>(service => service.DeleteFileAsync(mainImage));

                await transaction.CommitAsync();
                return responseMessage;
            }
            catch
            {
                if (transaction.GetDbTransaction().Connection is not null)
                    await transaction.RollbackAsync();
                return "AnErrorOccurredWhileDeletingTheChapter";
            }
        }
    }
}
