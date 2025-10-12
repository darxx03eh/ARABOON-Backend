using Araboon.Data.Entities;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Repositories
{
    public class ChapterRepository : GenericRepository<Chapter>, IChapterRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ChapterRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Chapter> GetChapterByMangaIdAndChapterNoAsync(int mangaId, int ChapterNo)
            => await GetTableNoTracking().Where(chapter => chapter.MangaID.Equals(mangaId) && chapter.ChapterNo.Equals(ChapterNo)).FirstOrDefaultAsync();

        public async Task<(string, IList<Chapter>?)> GetChaptersForSpecificMangaByLanguage(int mangaId, string language)
        {
            var isMangaExist = context.Mangas.Any(manga => manga.MangaID.Equals(mangaId));
            if (!isMangaExist)
                return ("MangaNotFound", null);
            var isLanguageExist = await IsLanguageExist(mangaId, language);
            if (!isLanguageExist)
                return ("TheLanguageYouRequestedIsNotAvailableForThisManga", null);
            var chapters = await GetTableNoTracking().Where(
                chapter => chapter.Language.ToLower().Equals(language.ToLower()) &&
                chapter.MangaID.Equals(mangaId)
                ).ToListAsync();
            if (chapters.Count.Equals(0))
                return ("ThereAreNoChaptersYet", null);
            return ("TheChaptersWereFound", chapters);
        }
        private async Task<bool> IsLanguageExist(int mangaId, string language)
        {
            if (language.ToLower().Equals("arabic"))
            {
                var isExist = await context.Mangas.Where(chapter => chapter.MangaID.Equals(mangaId))
                                    .Select(chapter => chapter.ArabicAvailable).FirstOrDefaultAsync();
                if (isExist is null)
                    return false;
                return (bool)isExist;
            }
            else
            {
                var isExist = await context.Mangas.Where(chapter => chapter.MangaID.Equals(mangaId))
                                    .Select(chapter => chapter.EnglishAvilable).FirstOrDefaultAsync();
                if (isExist is null)
                    return false;
                return (bool)isExist;
            }
        }
    }
}
