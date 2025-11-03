using Araboon.Data.DTOs.Chapters;
using Araboon.Data.Entities;
using Microsoft.AspNetCore.Http;

namespace Araboon.Service.Interfaces
{
    public interface IChapterService
    {
        public Task<(string, IList<Chapter>?)> GetChaptersForSpecificMangaByLanguage(int mangaId, string language);
        public Task<(string, int?)> ChapterReadAsync(int chapterId);
        public Task<(string, Chapter?)> AddNewChapterAsync(ChapterInfoDTO chapterInfo);
        public Task UploadChapterImagesAsync(int mangaId, int chapterNo, IList<string> images, string lang, int chapterId);
    }
}
