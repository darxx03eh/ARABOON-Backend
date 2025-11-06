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
        public Task<string> UploadChapterImagesAsync(int id, IList<IFormFile> images);
        public Task<string> DeleteExistingChapterAsync(int id);
        public Task<(string, Chapter?)> UpdateExistingChapterAsync(int id, int chapterNo, string arabicChapterTitle, string englishChapterTitle, string language);
        public Task<(string, string?)> UploadChapterImageAsync(int id, IFormFile image);
    }
}
