using Araboon.Data.Entities;
using Araboon.Data.Enums;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;

namespace Araboon.Infrastructure.Repositories
{
    public class MangaRepository : GenericRepository<Manga>, IMangaRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public MangaRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<(string, IList<HomePageResponse>?, IList<string>?)> GetCategoriesHomePageAsync()
        {
            string? userID = ExtractUserIdFromToken();
            IList<int> favoriteMangaIds = new List<int>();
            if (!string.IsNullOrEmpty(userID))
                favoriteMangaIds = await context.Favorites.Where(f => f.UserID.ToString().Equals(userID))
                                   .Select(f => f.MangaID).ToListAsync();
            var helper = await context.Categories.Where(c => c.IsActive).Select(c => new
            {
                en = c.CategoryNameEn,
                ar = c.CategoryNameAr
            }).ToListAsync();
            var mangasByCategory = new List<HomePageResponse>();
            foreach (var category in helper)
            {
                string en = category.en;
                string ar = category.ar;
                var mangas = await GetTableNoTracking()
                             .Where(manga => manga.CategoryMangas.Any(c => c.Category.CategoryNameEn.Equals(en)))
                             .OrderByDescending(manga => manga.Rate)
                             .Take(10).Select(manga => new GetCategoriesHomePageResponse()
                             {
                                 MangaID = manga.MangaID,
                                 MangaName = TransableEntity.GetTransable(manga.MangaNameEn, manga.MangaNameAr),
                                 MangaImageUrl = manga.MainImage,
                                 IsFavorite = favoriteMangaIds.Contains(manga.MangaID),
                                 LastChapter = manga.Chapters.OrderByDescending(chapter => chapter.ChapterNo)
                                 .Select(chapter => new LastChapter()
                                 {
                                     ChapterID = chapter.ChapterID,
                                     ChapterNo = chapter.ChapterNo,
                                     Views = chapter.ReadersCount
                                 }).FirstOrDefault()
                             }).ToListAsync();
                var Category = new CategoryResponse()
                {
                    En = en,
                    Ar = ar
                };
                mangasByCategory.Add(new HomePageResponse()
                {
                    Category = Category,
                    Mangas = mangas
                });
            }
            var categories = helper.Select(c => c.en).ToList();
            if (mangasByCategory is null || mangasByCategory.Count().Equals(0))
                return ("MangaNotFound", null, null);
            return ("MangaFound", mangasByCategory, categories);
        }
        public async Task<(string, IList<GetHottestMangasResponse>?)> GetHottestMangasAsync()
        {
            var hottestMangas = await GetTableNoTracking()
                                .OrderByDescending(manga => (manga.Rate * manga.RatingsCount))
                                .Select(manga => new GetHottestMangasResponse()
                                {
                                    MangaID = manga.MangaID,
                                    MangaName = TransableEntity.GetTransable(manga.MangaNameEn, manga.MangaNameAr),
                                    MangaImageUrl = manga.MainImage,
                                    AuthorName = TransableEntity.GetTransable(manga.AuthorEn, manga.AuthorAr),
                                    PopularityScore = (int?)(manga.Rate * manga.RatingsCount)
                                }).Take(10).ToListAsync();
            if (hottestMangas is null)
                return ("MangaNotFound", null);
            return ("MangaFound", hottestMangas);
        }
        public async Task<(string, PaginatedResult<MangaSearchResponse>?)> SearchAsync(string? search, int pageNumber, int pageSize)
        {
            var mangasQueryable = GetTableNoTracking().OrderByDescending(manga => manga.Rate * manga.RatingsCount).AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
                mangasQueryable = mangasQueryable.Where(
                manga => manga.MangaNameEn.ToLower().Contains(search.ToLower()) ||
                manga.MangaNameAr.ToLower().Contains(search.ToLower()) ||
                manga.AuthorEn.ToLower().Contains(search.ToLower()) ||
                manga.AuthorAr.ToLower().Contains(search.ToLower())
                );
            if (mangasQueryable is null)
                return ("MangaNotFound", null);
            string? userId = ExtractUserIdFromToken();
            IList<int> favoriteMangaIds = new List<int>();
            if (!string.IsNullOrEmpty(userId))
                favoriteMangaIds = await context.Favorites.Where(f => f.UserID.ToString().Equals(userId))
                                   .Select(f => f.MangaID).ToListAsync();
            var mangas = await mangasQueryable.Select(manga => new MangaSearchResponse()
            {
                MangaID = manga.MangaID,
                MangaName = TransableEntity.GetTransable(manga.MangaNameEn, manga.MangaNameAr),
                MangaImageUrl = manga.MainImage,
                AuthorName = TransableEntity.GetTransable(manga.AuthorEn, manga.AuthorAr),
                IsFavorite = favoriteMangaIds.Contains(manga.MangaID),
                LastChapter = manga.Chapters.OrderByDescending(chapter => chapter.ChapterNo)
                .Select(chapter => new LastChapter()
                {
                    ChapterID = chapter.ChapterID,
                    ChapterNo = chapter.ChapterNo,
                    Views = chapter.ReadersCount
                }).FirstOrDefault()
            }).ToPaginatedListAsync(pageNumber, pageSize);
            if (mangas.Data.Count().Equals(0))
                return ("MangaNotFound", null);
            return ("MangaFound", mangas);
        }
        public async Task<(string, PaginatedResult<GetPaginatedHottestMangaResponse>?)> GetPaginatedHottestMangaAsync(int pageNumber, int pageSize)
        {
            var hottestMangasQueryable = GetTableNoTracking().OrderByDescending(manga => (manga.Rate * manga.RatingsCount)).AsQueryable();
            if (hottestMangasQueryable is null)
                return ("MangaNotFound", null);
            var hottestMangas = await hottestMangasQueryable.Select(manga => new GetPaginatedHottestMangaResponse()
            {
                MangaID = manga.MangaID,
                MangaName = TransableEntity.GetTransable(manga.MangaNameEn, manga.MangaNameAr),
                MangaImageUrl = manga.MainImage,
                AuthorName = TransableEntity.GetTransable(manga.AuthorEn, manga.AuthorAr),
                PopularityScore = (int?)(manga.Rate * manga.RatingsCount)
            }).ToPaginatedListAsync(pageNumber, pageSize);
            if (hottestMangas.Data.Count().Equals(0))
                return ("MangaNotFound", null);
            return ("MangaFound", hottestMangas);
        }
        public async Task<(string, PaginatedResult<GetMangaByCategoryNameResponse>?)> GetMangaByCategoryNameAsync(string category, int pageNumber, int pageSize)
        {
            var mangasQueryable = GetTableNoTracking().Where(manga => manga.CategoryMangas.Any(
                c => c.Category.CategoryNameEn.ToLower()
                .Equals(category.ToLower()) || c.Category.CategoryNameAr.ToLower().Equals(category.ToLower())
                )).OrderByDescending(manga => manga.Rate).AsQueryable();
            if (mangasQueryable is null)
                return ("MangaNotFound", null);
            string? userID = ExtractUserIdFromToken();
            IList<int> favoriteMangaIds = new List<int>();
            if (!string.IsNullOrEmpty(userID))
                favoriteMangaIds = await context.Favorites.Where(f => f.UserID.ToString().Equals(userID))
                                   .Select(f => f.MangaID).ToListAsync();
            var mangas = await mangasQueryable.Select(manga => new GetMangaByCategoryNameResponse()
            {
                MangaID = manga.MangaID,
                MangaName = TransableEntity.GetTransable(manga.MangaNameEn, manga.MangaNameAr),
                MangaImageUrl = manga.MainImage,
                IsFavorite = favoriteMangaIds.Contains(manga.MangaID),
                AuthorName = TransableEntity.GetTransable(manga.AuthorEn, manga.AuthorAr),
                LastChapter = manga.Chapters.OrderByDescending(chapter => chapter.ChapterNo)
                .Select(chapter => new LastChapter()
                {
                    ChapterID = chapter.ChapterID,
                    ChapterNo = chapter.ChapterNo,
                    Views = chapter.ReadersCount
                }).FirstOrDefault()
            }).ToPaginatedListAsync(pageNumber, pageSize);
            if (mangas.Data.Count().Equals(0))
                return ("MangaNotFound", null);
            return ("MangaFound", mangas);
        }
        public async Task<(string, PaginatedResult<GetMangaByStatusResponse>?)> GetMangaByStatusAsync(int pageNumber, int pageSize, string status, MangaOrderingEnum orderBy, string? filter)
        {
            var mangaQueryable = GetTableNoTracking().Where(
                manga => manga.StatusEn.ToLower().Equals(status.ToLower()) ||
                manga.StatusAr.ToLower().Equals(status.ToLower())
                ).AsQueryable();
            if (mangaQueryable is null)
                return ("MangaNotFound", null);
            if (filter is not null)
                mangaQueryable = mangaQueryable.Where(
                    manga => manga.CategoryMangas.Any(
                        category => category.Category.CategoryNameEn.ToLower().Equals(filter.ToLower()) ||
                        category.Category.CategoryNameAr.ToLower().Equals(filter.ToLower())
                        )
                    );
            var isArabic = IsArabic();
            mangaQueryable = orderBy switch
            {
                MangaOrderingEnum.AtoZ => isArabic ?
                mangaQueryable.OrderBy(manga => manga.MangaNameAr) :
                mangaQueryable.OrderBy(manga => manga.MangaNameEn),
                MangaOrderingEnum.ZtoA => isArabic ?
                mangaQueryable.OrderByDescending(manga => manga.MangaNameAr) :
                mangaQueryable.OrderByDescending(manga => manga.MangaNameEn),
                MangaOrderingEnum.PopularityScore => mangaQueryable.OrderByDescending(manga => (manga.Rate * manga.RatingsCount)),
                _ => isArabic ?
                mangaQueryable.OrderBy(manga => manga.MangaNameAr) :
                mangaQueryable.OrderBy(manga => manga.MangaNameEn),
            };
            string? userID = ExtractUserIdFromToken();
            IList<int> favoriteMangaIds = new List<int>();
            if (!string.IsNullOrEmpty(userID))
                favoriteMangaIds = await context.Favorites.Where(f => f.UserID.ToString().Equals(userID))
                                   .Select(f => f.MangaID).ToListAsync();
            var mangas = await mangaQueryable.Select(manga => new GetMangaByStatusResponse()
            {
                MangaID = manga.MangaID,
                MangaName = TransableEntity.GetTransable(manga.MangaNameEn, manga.MangaNameAr),
                MangaImageUrl = manga.MainImage,
                IsFavorite = favoriteMangaIds.Contains(manga.MangaID),
                AuthorName = TransableEntity.GetTransable(manga.AuthorEn, manga.AuthorAr),
                LastChapter = manga.Chapters.OrderByDescending(chapter => chapter.ChapterNo)
                .Select(chapter => new LastChapter()
                {
                    ChapterID = chapter.ChapterID,
                    ChapterNo = chapter.ChapterNo,
                    Views = chapter.ReadersCount
                }).FirstOrDefault()
            }).ToPaginatedListAsync(pageNumber, pageSize);
            if (mangas.Data.Count().Equals(0))
                return ("MangaNotFound", null);
            return ("MangaFound", mangas);
        }
        private bool IsArabic()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var langHeader = httpContext?.Request.Headers["Accept-Language"].ToString();

            var lang = "en";
            if (!string.IsNullOrEmpty(langHeader))
                lang = langHeader.Split(',')[0].Split('-')[0];
            return lang.Equals("ar");
        }
    }
}
