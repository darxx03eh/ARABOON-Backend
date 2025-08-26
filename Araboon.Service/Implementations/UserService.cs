using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Users.Queries;
using Araboon.Infrastructure.Commons;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;

namespace Araboon.Service.Implementations
{
    internal class UserService : IUserService
    {
        private readonly UserManager<AraboonUser> userManager;
        private readonly RoleManager<AraboonRole> roleManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUnitOfWork unitOfWork;
        private readonly AraboonDbContext context;

        public UserService(UserManager<AraboonUser> userManager, RoleManager<AraboonRole> roleManager,
                           IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, AraboonDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.httpContextAccessor = httpContextAccessor;
            this.unitOfWork = unitOfWork;
            this.context = context;
        }

        public async Task<string> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            var userId = unitOfWork.FavoriteRepository.ExtractUserIdFromToken();
            if (String.IsNullOrEmpty(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var checkPassword = await userManager.CheckPasswordAsync(user, currentPassword);
                    if (!checkPassword)
                    {
                        await transaction.RollbackAsync();
                        return "TheCurrentPasswordIsWrong";
                    }
                    var deleteResult = await userManager.RemovePasswordAsync(user);
                    if (!deleteResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return "AnErrorOccurredWhileDeletingTheOldPassword";
                    }
                    var addResult = await userManager.AddPasswordAsync(user, newPassword);
                    if (!deleteResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return "AnErrorOccurredWhileAddingTheNewPassword";
                    }
                    await transaction.CommitAsync();
                    return "PasswordChangedSuccessfully";

                }
                catch (Exception exp)
                {
                    if (transaction.GetDbTransaction().Connection is not null)
                        await transaction.RollbackAsync();
                    return "AnErrorOccurredWhileChangingThePassword";
                }
            }
        }

        public async Task<string> ChangeUserNameAsync(string username)
        {
            var userId = unitOfWork.FavoriteRepository.ExtractUserIdFromToken();
            if (String.IsNullOrEmpty(userId))
                return "UserNotFound";

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";
            var result = await userManager.SetUserNameAsync(user, username);
            if (!result.Succeeded)
                return "AnErrorOccurredWhileChangingTheUsername";
            return "UsernameChangedSuccessfully";
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
                    CoverImage = new CoverImage()
                    {
                        OriginalImage = user.CoverImage.OriginalImage,
                        CroppedImage = user.CoverImage.CroppedImage,
                    },
                    ProfileImage = new ProfileImage()
                    {
                        OriginalImage = user.ProfileImage.OriginalImage,
                        CropData = new CropData()
                        {
                            Position = new Position()
                            {
                                X = user.ProfileImage.X,
                                Y = user.ProfileImage.Y,
                            },
                            Scale = user.ProfileImage.Scale,
                            Rotate = user.ProfileImage.Rotate
                        }
                    },
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
            catch (Exception exp)
            {
                return ("ThereWasAProblemLoadingTheProfile", null);
            }
        }
    }
}
