using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.ChapterImages;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Araboon.Service.Implementations
{
    public class ChapterImagesService : IChapterImagesService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AraboonUser> userManager;

        public ChapterImagesService(IUnitOfWork unitOfWork, UserManager<AraboonUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        public async Task<(string, ChapterImagesResponse?, string?, int?)> GetChapterImagesAsync(int mangaId, int chapterNo, string language)
        {
            var userId = unitOfWork.UserRepository.ExtractUserIdFromToken();
            var manga = await unitOfWork.MangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return ("MangaNotFound", null, null, null);
            var chapter = await unitOfWork.ChapterRepository.GetChapterByMangaIdAndChapterNoAsync(
                mangaId, chapterNo, language.ToLower().Equals("ar") ? "arabic":"english"
                );
            if (chapter is null)
                return ("ChapterNotFound", null, null, null);

            ChapterImagesResponse images = null;
            string mangaName = "";
            int chaptersCounts = 0;
            if (language.ToLower().Equals("ar"))
            {
                if (Convert.ToBoolean(!manga.ArabicAvailable))
                    return ("ChapterForArabicLanguageNotExist", null, null, null);
                images = new ChapterImagesResponse()
                {
                    ChapterId = chapter.ChapterID,
                    IsView = string.IsNullOrWhiteSpace(userId) ? false:
                    unitOfWork.ChapterViewRepository.GetTableNoTracking()
                    .Any(x => x.UserID.Equals(Convert.ToInt32(userId)) && x.ChapterID.Equals(chapter.ChapterID)),
                    IsArabic = Convert.ToBoolean(manga.ArabicAvailable),
                    IsEnglish = Convert.ToBoolean(manga.EnglishAvilable),
                    Images = chapter.ArabicChapterImages.OrderBy(image => image.OrderImage)
                             .Select(image => image.ImageUrl).ToList()
                };
                mangaName = manga.MangaNameAr;
            }
            else if (language.ToLower().Equals("en"))
            {
                if (Convert.ToBoolean(!manga.EnglishAvilable))
                    return ("ChapterForEnglishLanguageNotExist", null, null, null);
                images = new ChapterImagesResponse()
                {
                    ChapterId = chapter.ChapterID,
                    IsView = string.IsNullOrWhiteSpace(userId) ? false :
                    unitOfWork.ChapterViewRepository.GetTableNoTracking()
                    .Any(x => x.UserID.Equals(Convert.ToInt32(userId)) && x.ChapterID.Equals(chapter.ChapterID)),
                    IsArabic = Convert.ToBoolean(manga.ArabicAvailable),
                    IsEnglish = Convert.ToBoolean(manga.EnglishAvilable),
                    Images = chapter.EnglishChapterImages.OrderBy(image => image.OrderImage)
                             .Select(image => image.ImageUrl).ToList()
                };
                mangaName = manga.MangaNameEn;
            }
            else return ("ThisLanguageNotExist", null, null, null);
            if (images is null || images.Images.Count().Equals(0))
                return ("ImagesNotFound", null, null, null);
            chaptersCounts = manga.Chapters.Where(chapter => chapter.Language.ToLower().Equals(
                language.ToLower().Equals("ar") ? "arabic" : "english"
                )).Count();
            return ("ImagesFound", images, mangaName, chaptersCounts);
        }
    }
}
