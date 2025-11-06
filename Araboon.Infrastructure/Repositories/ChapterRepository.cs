using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Repositories
{
    public class ChapterRepository : GenericRepository<Chapter>, IChapterRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<AraboonUser> userManager;

        public ChapterRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AraboonUser> userManager)
            : base(context, httpContextAccessor, userManager)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        public async Task<Chapter> GetChapterByMangaIdAndChapterNoAsync(int mangaId, int ChapterNo, string lang)
            => await GetTableNoTracking().Where(
                chapter => chapter.MangaID.Equals(mangaId)&&
                chapter.ChapterNo.Equals(ChapterNo)&&
                chapter.Language.ToLower().Equals(lang)
                ).FirstOrDefaultAsync();

        public async Task<(string, IList<Chapter>?)> GetChaptersForSpecificMangaByLanguage(int mangaId, string language)
        {
            var isMangaExist = context.Mangas.Any(manga => manga.MangaID.Equals(mangaId));
            if (!isMangaExist)
                return ("MangaNotFound", null);
            var isLanguageExist = await IsLanguageExist(mangaId, language);
            if (!isLanguageExist && !await IsAdmin())
                return ("TheLanguageYouRequestedIsNotAvailableForThisManga", null);
            var lang = language.ToLower().Equals("ar") ? "arabic" : "english";
            var chapters = await GetTableNoTracking().Where(
                chapter => chapter.Language.ToLower().Equals(lang.ToLower()) &&
                chapter.MangaID.Equals(mangaId)
                ).ToListAsync();
            if (chapters.Count.Equals(0))
                return ("ThereAreNoChaptersYet", null);
            return ("TheChaptersWereFound", chapters);
        }

        public async Task<bool> isChapterNoExistAsync(int mangaId, int chapterNo, string lang, int? excludeChapterId = null)
        {
            var query = GetTableNoTracking().Where(
                chapter => chapter.MangaID.Equals(mangaId)
                && chapter.ChapterNo.Equals(chapterNo)
                && chapter.Language.ToLower().Equals(lang.ToLower())
            );

            if (excludeChapterId.HasValue)
                query = query.Where(chapter => !chapter.ChapterID.Equals(excludeChapterId));

            return await query.AnyAsync();
        }

        private async Task<bool> IsLanguageExist(int mangaId, string language)
        {
            if (language.ToLower().Equals("ar"))
            {
                var isExist = await context.Mangas.Where(chapter => chapter.MangaID.Equals(mangaId))
                                    .Select(chapter => chapter.ArabicAvailable).FirstOrDefaultAsync();
                if (isExist is null)
                    return false;
                return Convert.ToBoolean(isExist);
            }
            else if (language.ToLower().Equals("en"))
            {
                var isExist = await context.Mangas.Where(chapter => chapter.MangaID.Equals(mangaId))
                                    .Select(chapter => chapter.EnglishAvilable).FirstOrDefaultAsync();
                if (isExist is null)
                    return false;
                return Convert.ToBoolean(isExist);
            }
            else return false;
        }
    }
}
