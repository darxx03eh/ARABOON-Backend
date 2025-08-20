using Araboon.Data.Entities;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Repositories
{
    public class ChapterViewRepository : GenericRepository<ChapterView>, IChapterViewRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        public ChapterViewRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Boolean> IsMangaAndChapterExistForUser(Int32 mangaId, Int32 chapterId, Int32 userId)
        {
            var flag = context.ChapterViews.Any(
                c => c.ChapterID.Equals(chapterId) && c.MangaID.Equals(mangaId) && c.UserID.Equals(userId)
                );
            return flag;
        }
        public async Task<Boolean> IsChapterExistInManga(Int32 chapterId, Int32 mangaId)
        {
            var chapters = await context.Chapters.AsNoTracking()
                           .Where(chapter => chapter.MangaID.Equals(mangaId))
                           .Select(chapter => chapter.ChapterID).ToListAsync();
            return chapters.Contains(chapterId);
        }
    }
}
