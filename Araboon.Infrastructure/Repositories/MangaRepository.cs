using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Enums;
using Araboon.Data.Response.Categories.Queries;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Araboon.Infrastructure.Repositories
{
    public class MangaRepository : GenericRepository<Manga>, IMangaRepository
    {
        private readonly AraboonDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<AraboonUser> userManager;

        public MangaRepository(AraboonDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AraboonUser> userManager)
            : base(context, httpContextAccessor, userManager)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }
        public async Task<(string, IList<HomePageResponse>?, IList<string>?)> GetCategoriesHomePageAsync(bool isAdmin)
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
                             .Where(manga => isAdmin ? true : manga.IsActive)
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
        public async Task<(string, IList<GetHottestMangasResponse>?)> GetHottestMangasAsync(bool flag)
        {
            var hottestMangas = await GetTableNoTracking()
                                .OrderByDescending(manga => (manga.Rate * manga.RatingsCount))
                                .Where(manga => flag ? true : manga.IsActive)
                                .Select(manga => new GetHottestMangasResponse()
                                {
                                    MangaID = manga.MangaID,
                                    MangaName = TransableEntity.GetTransable(manga.MangaNameEn, manga.MangaNameAr),
                                    MangaImageUrl = manga.MainImage,
                                    AuthorName = TransableEntity.GetTransable(manga.AuthorEn, manga.AuthorAr),
                                    PopularityScore = (int?)(manga.Rate * manga.RatingsCount)
                                }).Take(10).ToListAsync();
            if (hottestMangas is null || hottestMangas.Count().Equals(0))
                return ("MangaNotFound", null);
            return ("MangaFound", hottestMangas);
        }
        public async Task<(string, PaginatedResult<MangaSearchResponse>?)> SearchAsync(string? search, int pageNumber, int pageSize, bool isAdmin)
        {
            var mangasQueryable = GetTableNoTracking().Where(manga => isAdmin ? true : manga.IsActive)
                .OrderByDescending(manga => manga.Rate * manga.RatingsCount).AsQueryable();
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
        public async Task<(string, PaginatedResult<GetPaginatedHottestMangaResponse>?)> GetPaginatedHottestMangaAsync(int pageNumber, int pageSize, bool isAdmin)
        {
            var hottestMangasQueryable = GetTableNoTracking().Where(manga => isAdmin ? true : manga.IsActive)
                .OrderByDescending(manga => (manga.Rate * manga.RatingsCount)).AsQueryable();
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
        public async Task<(string, PaginatedResult<GetMangaByCategoryNameResponse>?)> GetMangaByCategoryNameAsync(string category, int pageNumber, int pageSize, bool isAdmin)
        {
            var mangasQueryable = GetTableNoTracking()
                                  .Where(manga =>
                                      manga.CategoryMangas.Any(c =>
                                          (c.Category.CategoryNameEn.ToLower() == category.ToLower() ||
                                           c.Category.CategoryNameAr.ToLower() == category.ToLower()))
                                      && (isAdmin || manga.IsActive))
                                  .OrderByDescending(manga => manga.Rate)
                                  .AsQueryable();
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
        public async Task<(string, PaginatedResult<GetMangaByStatusResponse>?)> GetMangaByStatusAsync(int pageNumber, int pageSize, string status, MangaOrderingEnum orderBy, string? filter, bool isAdmin)
        {
            var mangaQueryable = GetTableNoTracking().Where(
                manga => (manga.StatusEn.ToLower().Equals(status.ToLower()) ||
                manga.StatusAr.ToLower().Equals(status.ToLower()))
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
            var mangas = await mangaQueryable.Where(manga => isAdmin ? true : manga.IsActive)
                .Select(manga => new GetMangaByStatusResponse()
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

        public async Task<(string, PaginatedResult<GetMangaCommentsResponse>?)> GetMangaCommentsAsync(int id, int pageNumber, int pageSize)
        {
            var commentQueryable = context.Comments.AsNoTracking().Where(c => c.MangaID.Equals(id))
                .OrderByDescending(c => c.UpdatedAt).AsQueryable();
            if (commentQueryable is null)
                return ("CommentsNotFound", null);
            string? userId = ExtractUserIdFromToken();
            IList<int> likes = new List<int>();
            if (!string.IsNullOrWhiteSpace(userId))
                likes = await context.CommentLikes.Where(x => x.UserId.Equals(int.Parse(userId))).Select(x => x.CommentId).ToListAsync();

            var pagedComments = await commentQueryable.ToPaginatedListAsync(pageNumber, pageSize);
            if (pagedComments.Data.Equals(0))
                return ("CommentsNotFound", null);

            var comments = pagedComments.Data.Select(x => new GetMangaCommentsResponse()
            {
                Id = x.CommentID,
                Content = x.Content,
                Since = IsArabic()
                        ? x.UpdatedAt.Humanize(culture: new CultureInfo("ar"))
                        : x.UpdatedAt.Humanize(culture: new CultureInfo("en")),
                Likes = x.Likes,
                IsLiked = likes.Contains(x.CommentID),
                replyCount = x.Replies.Where(r => r.CommentID.Equals(x.CommentID)).Count(),
                User = new User()
                {
                    Id = x.UserID,
                    Name = $"{x.User.FirstName} {x.User.LastName}",
                    UserName = x.User.UserName,
                    ProfileImage = new Araboon.Data.Response.Users.Queries.ProfileImage()
                    {
                        OriginalImage = x.User.ProfileImage.OriginalImage,
                        CropData = new Araboon.Data.Response.Users.Queries.CropData()
                        {
                            Position = new Araboon.Data.Response.Users.Queries.Position()
                            {
                                X = x.User.ProfileImage.X,
                                Y = x.User.ProfileImage.Y,
                            },
                            Scale = x.User.ProfileImage.Scale,
                            Rotate = x.User.ProfileImage.Rotate,
                        }
                    },
                }
            }).ToList();
            var result = PaginatedResult<GetMangaCommentsResponse>.Success(
                comments,
                pagedComments.TotalCount,
                pagedComments.TotalPages,
                pagedComments.PageSize
                );
            result.CurrentPage = pagedComments.CurrentPage;
            if (result.Data.Count.Equals(0))
                return ("CommentsNotFound", null);
            return ("CommentsFound", result);
        }

        public async Task<int> CommentsCountByIdAsync(int id)
            => await context.Comments.Where(comment => comment.MangaID.Equals(id)).CountAsync();

        public async Task<bool> IsMangaNameArExist(string en)
            => await GetTableNoTracking()
            .Where(manga => manga.MangaNameAr.ToLower().Equals(en.ToLower())).FirstOrDefaultAsync() is not null;

        public async Task<bool> IsMangaNameEnExist(string ar)
            => await GetTableNoTracking()
            .Where(manga => manga.MangaNameEn.ToLower().Equals(ar.ToLower())).FirstOrDefaultAsync() is not null;

        public async Task<(string, PaginatedResult<GetMangaForDashboardResponse>?)> GetMangaForDashboardAsync(string? search, int pageNumber, int pageSize, bool isAdmin)
        {
            var mangasQueryable = GetTableNoTracking().Where(manga => isAdmin ? true : manga.IsActive)
                .OrderByDescending(manga => manga.Rate * manga.RatingsCount).AsQueryable();
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
            var mangas = await mangasQueryable.Select(manga => new GetMangaForDashboardResponse()
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
                }).FirstOrDefault(),
                Name = new MangaName() { En = manga.MangaNameEn, Ar = manga.MangaNameAr },
                Description = new Description() { En = manga.DescriptionEn, Ar = manga.DescriptionAr },
                Author = new Author() { En = manga.AuthorEn, Ar = manga.AuthorAr },
                Type = new Araboon.Data.Response.Mangas.Queries.Type() { En = manga.TypeEn, Ar = manga.TypeAr },
                Status = new Status() { En = manga.StatusEn, Ar = manga.StatusAr },
                Categories = manga.CategoryMangas.Select(category => new CategoriesResponse()
                {
                    Id = category.CategoryID,
                    En = category.Category.CategoryNameEn,
                    Ar = category.Category.CategoryNameAr
                }).ToList(),
                IsActive = manga.IsActive,
                IsArabicAvailable = Convert.ToBoolean(manga.ArabicAvailable),
                IsEnglishAvailable = Convert.ToBoolean(manga.EnglishAvilable)
            }).ToPaginatedListAsync(pageNumber, pageSize);
            if (mangas.Data.Count().Equals(0))
                return ("MangaNotFound", null);
            return ("MangaFound", mangas);
        }
    }
}
