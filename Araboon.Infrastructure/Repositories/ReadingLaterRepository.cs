using Araboon.Data.Entities;
using Araboon.Data.Response.CurrentlyReadings.Queries;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Response.ReadingLaters.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Infrastructure.Repositories
{
    public class ReadingLaterRepository : GenericRepository<ReadingLater>, IReadingLaterRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        public ReadingLaterRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor) 
            : base(context, httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<(String, PaginatedResult<GetPaginatedReadingLaterMangaResponse>?)> GetPaginatedReadingLaterMangaAsync(Int32 pageNumber, Int32 pageSize)
        {
            String? userId = ExtractUserIdFromToken();
            if (String.IsNullOrEmpty(userId))
                return ("ReadingLaterServiceforRegisteredUsersOnly", null);
            IList<Int32> favoriteMangaIds = new List<Int32>();
            favoriteMangaIds = await context.Favorites.Where(f => f.UserID.ToString().Equals(userId))
                               .Select(f => f.MangaID).ToListAsync();
            var currentlyReadingsManga = GetTableNoTracking().Where(r => r.UserID.Equals(Int32.Parse(userId)))
                                 .OrderByDescending(r => r.Manga.Rate).AsQueryable();
            if (currentlyReadingsManga is null)
                return ("ThereAreNoMangaInYourReadingLaterList", null);
            var mangas = await currentlyReadingsManga.Select(r => new GetPaginatedReadingLaterMangaResponse()
            {
                MangaID = r.MangaID,
                MangaName = TransableEntity.GetTransable(r.Manga.MangaNameEn, r.Manga.MangaNameAr),
                MangaImageUrl = r.Manga.MainImage,
                IsFavorite = favoriteMangaIds.Contains(r.MangaID),
                AuthorName = TransableEntity.GetTransable(r.Manga.AuthorEn, r.Manga.AuthorAr),
                LastChapter = r.Manga.Chapters.OrderByDescending(chapter => chapter.ChapterNo)
                .Select(chapter => new LastChapter()
                {
                    ChapterNo = chapter.ChapterNo,
                    ChapterID = chapter.ChapterID,
                    Views = chapter.ReadersCount
                }).FirstOrDefault()
            }).ToPaginatedListAsync(pageNumber, pageSize);
            if (mangas.Data.Count().Equals(0))
                return ("ThereAreNoMangaInYourReadingLaterList", null);
            return ("TheMangaWasFoundInYourReadingLaterList", mangas);
        }
        public async Task<Boolean> IsMangaExistForUser(Int32 mangaId, Int32 userId)
        {
            var manga = context.ReadingLaters.Any(
                r => r.MangaID.Equals(mangaId) && r.UserID.Equals(userId)
                );
            return manga;
        }
    }
}
