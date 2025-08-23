using Araboon.Data.Entities;

namespace Araboon.Infrastructure.IRepositories
{
    public interface IChapterViewRepository : IGenericRepository<ChapterView>
    {
        public Task<bool> IsMangaAndChapterExistForUser(int mangaId, int chapterId, int userId);
        public Task<bool> IsChapterExistInManga(int chapterId, int mangaId);
    }
}
