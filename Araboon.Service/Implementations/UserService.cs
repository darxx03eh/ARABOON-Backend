using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Users.Queries;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Araboon.Service.Implementations
{
    internal class UserService : IUserService
    {
        private readonly UserManager<AraboonUser> userManager;
        private readonly RoleManager<AraboonRole> roleManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUnitOfWork unitOfWork;

        public UserService(UserManager<AraboonUser> userManager, RoleManager<AraboonRole> roleManager,
                           IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.httpContextAccessor = httpContextAccessor;
            this.unitOfWork = unitOfWork;
        }
        public async Task<(string, UserProfileResponse?)> GetUserProfileAsync(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user is null)
                return ("UserNotFound", null);
            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                var langHeader = httpContext?.Request.Headers["Accept-Language"].ToString();

                var lang = "en";
                if (!string.IsNullOrEmpty(langHeader))
                    lang = langHeader.Split(',')[0].Split('-')[0];
                var culture = lang == "ar" ? new CultureInfo("ar") : new CultureInfo("en");
                var profile = new UserProfileResponse()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = $"@{user.UserName}",
                    Email = user.Email,
                    ImageUrl = user.ProfileImage,
                    CoverImageUrl = user.CoverImage,
                    JoinDate = user.CreatedAt.ToString(
                        culture.TwoLetterISOLanguageName == "ar" ? "dd MMMM yyyy" : "MMMM dd, yyyy"
                        , culture),
                    Role = string.Join(", ", await userManager.GetRolesAsync(user)),
                    IsActive = user.IsActive,
                    Bio = user.Bio,
                    Library = new Library()
                    {
                        FavoritesCount = await unitOfWork.FavoriteRepository.GetTableNoTracking().Where(
                            f => f.UserID.Equals(user.Id)
                            ).CountAsync(),
                        CompletedReadsCount = await unitOfWork.CompletedReadsRepository.GetTableNoTracking().Where(
                            c => c.UserID.Equals(user.Id)
                            ).CountAsync(),
                        ReadingLatersCount = await unitOfWork.ReadingLaterRepository.GetTableNoTracking().Where(
                            r => r.UserID.Equals(user.Id)
                            ).CountAsync(),
                        CurrentlyReadingCount = await unitOfWork.CurrentlyReadingRepository.GetTableNoTracking().Where(
                            c => c.UserID.Equals(user.Id)
                            ).CountAsync(),
                    },
                    FavoritesCategories = new List<FavoritesCategory>()
                };
                var (message, categories) = await unitOfWork.CategoryRepository.GetCategoriesAsync();
                if (message.Equals("CategoriesFound"))
                {
                    foreach (var category in categories)
                    {
                        var temp = new FavoritesCategory()
                        {
                            Category = TransableEntity.GetTransable(category.CategoryNameEn, category.CategoryNameAr),
                            Count = await unitOfWork.FavoriteRepository.GetTableNoTracking().Where(
                                f => f.UserID.Equals(user.Id) && f.Manga.CategoryMangas.Any(c => c.Category.CategoryNameEn.Equals(category))
                                ).CountAsync()
                        };
                        profile.FavoritesCategories.Add(temp);
                    }
                }
                return ("UserFound", profile);
            }
            catch(Exception exp)
            {
                return ("ThereWasAProblemLoadingTheProfile", null);
            }
        }
    }
}
