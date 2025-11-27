using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.ChapterImages;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Araboon.Service.Implementations
{
    public class ChapterImagesService : IChapterImagesService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AraboonUser> userManager;
        private readonly ILogger<ChapterImagesService> logger;

        public ChapterImagesService(IUnitOfWork unitOfWork, UserManager<AraboonUser> userManager, ILogger<ChapterImagesService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<(string, ChapterImagesResponse?, string?, int?)> GetChapterImagesAsync(int mangaId, int chapterNo, string language)
        {
            logger.LogInformation("Fetching chapter images - جلب صور الفصل | MangaId: {MangaId}, ChapterNo: {ChapterNo}, Language: {Lang}", mangaId, chapterNo, language);

            var userId = unitOfWork.UserRepository.ExtractUserIdFromToken();

            var manga = await unitOfWork.MangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning("Manga not found - لم يتم العثور على المانجا | MangaId: {MangaId}", mangaId);
                return ("MangaNotFound", null, null, null);
            }

            logger.LogInformation("Fetching chapter details - جلب بيانات الفصل | MangaId: {MangaId}, ChapterNo: {ChapterNo}", mangaId, chapterNo);

            var chapter = await unitOfWork.ChapterRepository.GetChapterByMangaIdAndChapterNoAsync(
                mangaId,
                chapterNo,
                language.ToLower().Equals("ar") ? "arabic" : "english"
            );

            if (chapter is null)
            {
                logger.LogWarning("Chapter not found - لم يتم العثور على الفصل | MangaId: {MangaId}, ChapterNo: {ChapterNo}", mangaId, chapterNo);
                return ("ChapterNotFound", null, null, null);
            }

            ChapterImagesResponse images = null;
            string mangaName = "";
            int chaptersCounts = 0;

            if (language.ToLower().Equals("ar"))
            {
                logger.LogInformation("Processing Arabic chapter images - معالجة صور الفصل العربي | MangaId: {MangaId}, ChapterId: {ChapterId}", mangaId, chapter.ChapterID);

                if (Convert.ToBoolean(!manga.ArabicAvailable) && !await unitOfWork.ChapterRepository.IsAdmin())
                {
                    logger.LogWarning("Arabic chapter not available - الفصل العربي غير متاح | MangaId: {MangaId}", mangaId);
                    return ("ChapterForArabicLanguageNotExist", null, null, null);
                }

                images = new ChapterImagesResponse()
                {
                    ChapterId = chapter.ChapterID,
                    IsView = string.IsNullOrWhiteSpace(userId) ? false :
                              unitOfWork.ChapterViewRepository.GetTableNoTracking()
                              .Any(x => x.UserID.Equals(Convert.ToInt32(userId)) && x.ChapterID.Equals(chapter.ChapterID)),
                    IsArabic = Convert.ToBoolean(manga.ArabicAvailable) || manga.Chapters.Any(c => c.Language.ToLower().Equals("arabic")),
                    IsEnglish = Convert.ToBoolean(manga.EnglishAvilable) || manga.Chapters.Any(c => c.Language.ToLower().Equals("english")),
                    Images = chapter.ArabicChapterImages
                             .OrderBy(image => image.OrderImage)
                             .Select(image => image.ImageUrl)
                             .ToList()
                };

                mangaName = manga.MangaNameAr;
            }
            else if (language.ToLower().Equals("en"))
            {
                logger.LogInformation("Processing English chapter images - معالجة صور الفصل الإنجليزي | MangaId: {MangaId}, ChapterId: {ChapterId}", mangaId, chapter.ChapterID);

                if (Convert.ToBoolean(!manga.EnglishAvilable) && !await unitOfWork.ChapterRepository.IsAdmin())
                {
                    logger.LogWarning("English chapter not available - الفصل الإنجليزي غير متاح | MangaId: {MangaId}", mangaId);
                    return ("ChapterForEnglishLanguageNotExist", null, null, null);
                }

                images = new ChapterImagesResponse()
                {
                    ChapterId = chapter.ChapterID,
                    IsView = string.IsNullOrWhiteSpace(userId) ? false :
                              unitOfWork.ChapterViewRepository.GetTableNoTracking()
                              .Any(x => x.UserID.Equals(Convert.ToInt32(userId)) && x.ChapterID.Equals(chapter.ChapterID)),
                    IsArabic = Convert.ToBoolean(manga.ArabicAvailable) || manga.Chapters.Any(c => c.Language.ToLower().Equals("arabic")),
                    IsEnglish = Convert.ToBoolean(manga.EnglishAvilable) || manga.Chapters.Any(c => c.Language.ToLower().Equals("english")),
                    Images = chapter.EnglishChapterImages
                             .OrderBy(image => image.OrderImage)
                             .Select(image => image.ImageUrl)
                             .ToList()
                };

                mangaName = manga.MangaNameEn;
            }
            else
            {
                logger.LogWarning("Invalid language - لغة غير صحيحة | Language: {Lang}", language);
                return ("ThisLanguageNotExist", null, null, null);
            }

            if (images is null || images.Images.Count().Equals(0))
            {
                logger.LogWarning("Images not found - لم يتم العثور على الصور | MangaId: {MangaId}, ChapterId: {ChapterId}", mangaId, chapter.ChapterID);
                return ("ImagesNotFound", null, null, null);
            }

            chaptersCounts = manga.Chapters
                .Where(c => c.Language.ToLower().Equals(language.ToLower().Equals("ar") ? "arabic" : "english"))
                .Count();

            logger.LogInformation("Chapter images retrieved successfully - تم جلب صور الفصل بنجاح | MangaId: {MangaId}, ChapterId: {ChapterId}", mangaId, chapter.ChapterID);

            return ("ImagesFound", images, mangaName, chaptersCounts);
        }
    }
}