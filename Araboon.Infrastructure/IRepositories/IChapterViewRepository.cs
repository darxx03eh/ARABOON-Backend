using Araboon.Data.Entities;

namespace Araboon.Infrastructure.IRepositories
{
    public interface IChapterViewRepository : IGenericRepository<ChapterView>
    {
        public Task<Boolean> IsMangaAndChapterExistForUser(Int32 mangaId, Int32 chapterId, Int32 userId);
        public Task<Boolean> IsChapterExistInManga(Int32 chapterId, Int32 mangaId);
    }
}
