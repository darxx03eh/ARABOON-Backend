using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Dashboards.Queries;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Araboon.Service.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<AraboonUser> userManager;
        private readonly ILogger<DashboardService> logger;

        public DashboardService(IUnitOfWork unitOfWork, UserManager<AraboonUser> userManager, ILogger<DashboardService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task<(string, DashboardStatisticsResponse)> DashboardStatisticsAsync()
        {
            logger.LogInformation("Fetching dashboard statistics - جلب إحصائيات لوحة التحكم");

            var totalCategories = await unitOfWork.CategoryRepository.GetTableNoTracking().CountAsync();
            var totalMangas = await unitOfWork.MangaRepository.GetTableNoTracking().CountAsync();
            var totalUsers = await userManager.Users.CountAsync();
            var totalBanners = await unitOfWork.SwiperRepository.GetTableNoTracking().CountAsync();

            logger.LogInformation("Counts retrieved - تم جلب الأعداد | Categories: {Cat}, Mangas: {Man}, Users: {Users}, Banners: {Ban}",
                totalCategories, totalMangas, totalUsers, totalBanners);

            var now = DateTime.UtcNow;
            var thisWeekStart = now.AddDays(-7);
            var lastWeekStart = now.AddDays(-14);
            var response = new DashboardStatisticsResponse();

            #region Users

            logger.LogInformation("Processing user statistics - معالجة إحصائيات المستخدمين");

            var usersThisWeek = await userManager.Users.CountAsync(user => user.CreatedAt >= thisWeekStart);
            var usersLastWeek = await userManager.Users.CountAsync(user => user.CreatedAt >= lastWeekStart && user.CreatedAt < thisWeekStart);
            var usersDiff = usersThisWeek - usersLastWeek;
            var usersPercentage = usersLastWeek.Equals(0) ? (usersThisWeek > 0 ? 100.0 : 0.0)
                                                         : Convert.ToDouble(usersDiff) / usersLastWeek * 100;

            response.Users = new DashboardStatistics()
            {
                TotalCounts = totalUsers,
                IsRise = usersDiff > 0,
                Percentage = usersPercentage,
            };

            logger.LogInformation("User stats calculated - تم حساب إحصائيات المستخدمين | Rise: {Rise}, Percentage: {P}",
                response.Users.IsRise, response.Users.Percentage);

            #endregion Users

            #region Categories

            logger.LogInformation("Processing category statistics - معالجة إحصائيات التصنيفات");

            var categoriesThisWeek = await unitOfWork.CategoryRepository.GetTableNoTracking()
                                   .CountAsync(category => category.CreatedAt >= thisWeekStart);
            var categoriesLastWeek = await unitOfWork.CategoryRepository.GetTableNoTracking()
                                     .CountAsync(category => category.CreatedAt >= lastWeekStart && category.CreatedAt < thisWeekStart);
            var categoriesDiff = categoriesThisWeek - categoriesLastWeek;
            var categoriesPercentage = categoriesLastWeek.Equals(0) ? (categoriesThisWeek > 0 ? 100.0 : 0.0)
                                                                    : Convert.ToDouble(categoriesDiff) / categoriesLastWeek * 100;

            response.Categories = new DashboardStatistics()
            {
                TotalCounts = totalCategories,
                IsRise = categoriesDiff > 0,
                Percentage = categoriesPercentage,
            };

            logger.LogInformation("Category stats calculated - تم حساب إحصائيات التصنيفات");

            #endregion Categories

            #region Mangas

            logger.LogInformation("Processing manga statistics - معالجة إحصائيات المانجا");

            var mangasThisWeek = await unitOfWork.MangaRepository.GetTableNoTracking().CountAsync(manga => manga.CreatedAt >= thisWeekStart);
            var mangasLastWeek = await unitOfWork.MangaRepository.GetTableNoTracking()
                                 .CountAsync(manga => manga.CreatedAt >= lastWeekStart && manga.CreatedAt < thisWeekStart);
            var mangasDiff = mangasThisWeek - mangasLastWeek;
            var mangasPercentage = mangasLastWeek.Equals(0) ? (mangasThisWeek > 0 ? 100.0 : 0.0)
                                                            : Convert.ToDouble(mangasDiff) / mangasLastWeek * 100;

            response.Mangas = new DashboardStatistics()
            {
                TotalCounts = totalMangas,
                IsRise = mangasDiff > 0,
                Percentage = mangasPercentage,
            };

            logger.LogInformation("Manga stats calculated - تم حساب إحصائيات المانجا");

            #endregion Mangas

            #region Banners

            logger.LogInformation("Processing banner statistics - معالجة إحصائيات البنرات");

            var bannersThisWeek = await unitOfWork.SwiperRepository.GetTableNoTracking()
                                   .CountAsync(banner => banner.CreatedAt >= thisWeekStart);
            var bannersLastWeek = await unitOfWork.SwiperRepository.GetTableNoTracking()
                                  .CountAsync(banner => banner.CreatedAt >= lastWeekStart && banner.CreatedAt < thisWeekStart);
            var bannersDiff = bannersThisWeek - bannersLastWeek;
            var bannersPercentage = bannersLastWeek.Equals(0) ? (bannersThisWeek > 0 ? 100.0 : 0.0)
                                                              : Convert.ToDouble(bannersDiff) / bannersLastWeek * 100;

            response.Banners = new DashboardStatistics()
            {
                TotalCounts = totalBanners,
                IsRise = bannersDiff > 0,
                Percentage = bannersPercentage,
            };

            logger.LogInformation("Banner stats calculated - تم حساب إحصائيات البنرات");

            #endregion Banners

            #region Top Categories

            logger.LogInformation("Fetching top categories - جلب أفضل التصنيفات");

            var categoryMangas = await unitOfWork.CategoryMangaRepository.GetTableNoTracking().ToListAsync();
            IList<TopCategories> topCategoriesList = new List<TopCategories>();
            var topCategories = await unitOfWork.CategoryRepository.GetTableNoTracking().ToListAsync();

            if (topCategories is not null)
            {
                foreach (var category in topCategories)
                {
                    topCategoriesList.Add(new TopCategories()
                    {
                        Name = TransableEntity.GetTransable(category.CategoryNameEn, category.CategoryNameAr),
                        TotalMangasCount = categoryMangas.Count(c => c.CategoryID.Equals(category.CategoryID))
                    });
                }
            }

            response.TopCategories = topCategoriesList;

            logger.LogInformation("Top categories generated - تم إنشاء قائمة أفضل التصنيفات");

            #endregion Top Categories

            #region Manga Ratio

            logger.LogInformation("Calculating manga ratios - حساب نسب المانجا");

            var activeCount = await unitOfWork.MangaRepository.GetTableNoTracking().CountAsync(manga => manga.IsActive);
            var activeRatio = totalMangas == 0 ? 0 : Convert.ToDouble(activeCount) / totalMangas * 100;
            var inactiveRatio = totalMangas == 0 ? 0 : 100 - activeRatio;

            response.MangaPercentage = new MangaPercentage()
            {
                ActivePercentage = activeRatio,
                InActivePercentage = inactiveRatio
            };

            logger.LogInformation("Manga ratio calculations completed - تم حساب نسب المانجا");

            #endregion Manga Ratio

            logger.LogInformation("Dashboard statistics completed successfully - تم جلب الإحصائيات بنجاح");

            return ("DashboardStatisticsRetrievedSuccessfully", response);
        }
    }
}