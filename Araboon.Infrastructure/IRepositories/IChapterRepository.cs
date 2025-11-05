using Araboon.Data.Entities;

namespace Araboon.Infrastructure.IRepositories
{
    public interface IChapterRepository : IGenericRepository<Chapter>
    {
        public Task<(string, IList<Chapter>?)> GetChaptersForSpecificMangaByLanguage(int mangaId, string language);
        public Task<Chapter> GetChapterByMangaIdAndChapterNoAsync(int mangaId, int ChapterNo, string lang);
        public Task<bool> isChapterNoExistAsync(int mangaId, int chapterNo, string lang);
    }
}
