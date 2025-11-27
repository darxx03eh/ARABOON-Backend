using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Dashboards.Queries;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Araboon.Service.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AraboonUser> userManager;

        public DashboardService(IUnitOfWork unitOfWork, UserManager<AraboonUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        public async Task<(string, DashboardStatisticsResponse)> DashboardStatisticsAsync()
        {
            var totalCategories = await unitOfWork.CategoryRepository.GetTableNoTracking().CountAsync();
            var totalMangas = await unitOfWork.MangaRepository.GetTableNoTracking().CountAsync();
            var totalUsers = await userManager.Users.CountAsync();
            var totalBanners = await unitOfWork.SwiperRepository.GetTableNoTracking().CountAsync();

            var now = DateTime.UtcNow;
            var thisWeekStart = now.AddDays(-7);
            var lastWeekStart = now.AddDays(-14);
            var response = new DashboardStatisticsResponse();

            #region Users

            var usersThisWeek = await userManager.Users.CountAsync(user => user.CreatedAt >= thisWeekStart);
            var usersLastWeek = await userManager.Users.CountAsync(user => user.CreatedAt >= lastWeekStart && user.CreatedAt < thisWeekStart);
            var usersDiff = usersThisWeek - usersLastWeek;
            var usersPercentage = usersLastWeek.Equals(0) ? (usersThisWeek > 0 ? 100.0 : 0.0)
                                                         : Convert.ToDouble(usersDiff) / usersLastWeek * 100;

            var users = new DashboardStatistics()
            {
                TotalCounts = totalUsers,
                IsRise = usersDiff > 0,
                Percentage = usersPercentage,
            };
            response.Users = users;

            #endregion Users

            #region Categories

            var categoriesThisWeek = await unitOfWork.CategoryRepository.GetTableNoTracking()
                                   .CountAsync(category => category.CreatedAt >= thisWeekStart);
            var categoriesLastWeek = await unitOfWork.CategoryRepository.GetTableNoTracking()
                                     .CountAsync(category => category.CreatedAt >= lastWeekStart && category.CreatedAt < thisWeekStart);
            var categoriesDiff = categoriesThisWeek - categoriesLastWeek;
            var categoriesPercentage = categoriesLastWeek.Equals(0) ? (categoriesThisWeek > 0 ? 100.0 : 0.0)
                                                                    : Convert.ToDouble(categoriesDiff) / categoriesLastWeek * 100;
            var categories = new DashboardStatistics()
            {
                TotalCounts = totalCategories,
                IsRise = categoriesDiff > 0,
                Percentage = categoriesPercentage,
            };
            response.Categories = categories;

            #endregion Categories

            #region Mangas

            var mangasThisWeek = await unitOfWork.MangaRepository.GetTableNoTracking().CountAsync(manga => manga.CreatedAt >= thisWeekStart);
            var mangasLastWeek = await unitOfWork.MangaRepository.GetTableNoTracking()
                                 .CountAsync(manga => manga.CreatedAt >= lastWeekStart && manga.CreatedAt < thisWeekStart);
            var mangasDiff = mangasThisWeek - mangasLastWeek;
            var mangasPercentage = mangasLastWeek.Equals(0) ? (mangasThisWeek > 0 ? 100.0 : 0.0)
                                                            : Convert.ToDouble(mangasDiff) / mangasLastWeek * 100;
            var mangas = new DashboardStatistics()
            {
                TotalCounts = totalMangas,
                IsRise = mangasDiff > 0,
                Percentage = mangasPercentage,
            };
            response.Mangas = mangas;

            #endregion Mangas

            #region Banners

            var bannersThisWeek = await unitOfWork.SwiperRepository.GetTableNoTracking().CountAsync(banner => banner.CreatedAt >= thisWeekStart);
            var bannersLastWeek = await unitOfWork.SwiperRepository.GetTableNoTracking()
                                  .CountAsync(banner => banner.CreatedAt >= lastWeekStart && banner.CreatedAt < thisWeekStart);
            var bannersDiff = bannersThisWeek - bannersLastWeek;
            var bannersPercentage = bannersLastWeek.Equals(0) ? (bannersThisWeek > 0 ? 100.0 : 0.0)
                                                              : Convert.ToDouble(bannersDiff) / bannersLastWeek * 100;
            var banners = new DashboardStatistics()
            {
                TotalCounts = totalBanners,
                IsRise = bannersDiff > 0,
                Percentage = bannersPercentage,
            };
            response.Banners = banners;

            #endregion Banners

            #region Top Categories

            var categoryMangas = await unitOfWork.CategoryMangaRepository.GetTableNoTracking().ToListAsync();
            IList<TopCategories> topCategoriesList = new List<TopCategories>();
            var topCategories = await unitOfWork.CategoryRepository.GetTableNoTracking().ToListAsync();
            if (topCategories is not null)
            {
                foreach (var category in topCategories)
                {
                    var topCategory = new TopCategories()
                    {
                        Name = TransableEntity.GetTransable(category.CategoryNameEn, category.CategoryNameAr),
                        TotalMangasCount = categoryMangas.Count(c => c.CategoryID.Equals(category.CategoryID))
                    };
                    topCategoriesList.Add(topCategory);
                }
            }
            response.TopCategories = topCategoriesList;

            #endregion Top Categories

            #region Mangas Ratio

            var activeRatio = 0.0;
            var inactiveRatio = 0.0;
            if (totalMangas.Equals(0))
                (activeRatio, inactiveRatio) = (0, 0);

            var activeCount = await unitOfWork.MangaRepository.GetTableNoTracking().CountAsync(manga => manga.IsActive);
            activeRatio = Convert.ToDouble(activeCount) / totalMangas * 100;
            inactiveRatio = 100 - activeRatio;
            response.MangaPercentage = new MangaPercentage()
            {
                ActivePercentage = activeRatio,
                InActivePercentage = inactiveRatio
            };

            #endregion Mangas Ratio

            return ("DashboardStatisticsRetrievedSuccessfully", response);
        }
    }
}