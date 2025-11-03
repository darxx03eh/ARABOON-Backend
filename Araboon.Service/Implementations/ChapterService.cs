using Araboon.Data.DTOs.Chapters;
using Araboon.Data.Entities;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Http;
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
            var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if(chapterInfo.Image is not null)
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
                    using var stream = System.IO.File.Create(tempPath);
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
                    chapterInfo.Language.ToLower().Equals("ar") ? $"{domain}/manga/{chapterInfo.MangaId}/chapter/{chapterInfo.ChapterNo}?lang=ar" 
                                                                : $"{domain}/manga/{chapterInfo.MangaId}/chapter/{chapterInfo.ChapterNo}?lang=en",
                    data
                ));
                await transaction.CommitAsync();
                return ("ChapterAddedSuccessfully", result);
            }catch(Exception exp)
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
            var folderName = $"ARABOON/Mangas/{mangaId}/Chapters/{chapterNo}-{language}";
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
                var folderName = $"ARABOON/Mangas/{mangaId}/Chapters/{chapterNo}-{language}/img";
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
    }
}
