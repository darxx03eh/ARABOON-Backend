using Araboon.Data.Entities;

namespace Araboon.Service.Interfaces
{
    public interface IChapterService
    {
        public Task<(string, IList<Chapter>?)> GetChaptersForSpecificMangaByLanguage(int mangaId, string language);
        public Task<(string, int?)> ChapterReadAsync(int chapterId);
    }
}
