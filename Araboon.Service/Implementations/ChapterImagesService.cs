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

        public async Task<(string, ChapterImagesResponse?)> GetChapterImagesAsync(int mangaId, int chapterNo, string language)
        {

            var manga = await unitOfWork.MangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return ("MangaNotFound", null);

            var chapter = await unitOfWork.ChapterRepository.GetChapterByMangaIdAndChapterNoAsync(mangaId, chapterNo);
            if (chapter is null)
                return ("ChapterNotFound", null);

            ChapterImagesResponse images = null;
            if (language.ToLower().Equals("ar"))
            {
                if (Convert.ToBoolean(!manga.ArabicAvailable))
                    return ("ChapterForArabicLanguageNotExist", null);
                images = new ChapterImagesResponse()
                {
                    ChapterId = chapter.ChapterID,
                    IsArabic = true,
                    IsEnglish = false,
                    Images = chapter.ArabicChapterImages.OrderBy(image => image.OrderImage)
                             .Select(image => image.ImageUrl).ToList()
                };
            }
            else
            {
                if (Convert.ToBoolean(!manga.EnglishAvilable))
                    return ("ChapterForEnglishLanguageNotExist", null);
                images = new ChapterImagesResponse()
                {
                    ChapterId = chapter.ChapterID,
                    IsArabic = false,
                    IsEnglish = true,
                    Images = chapter.EnglishChapterImages.OrderBy(image => image.OrderImage)
                             .Select(image => image.ImageUrl).ToList()
                };
            }
            if (images is null || images.Images.Count().Equals(0))
                return ("ImagesNotFound", null);

            return ("ImagesFound", images);
        }
    }
}
