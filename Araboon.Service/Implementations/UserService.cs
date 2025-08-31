using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Users.Queries;
using Araboon.Data.Routing;
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
        private readonly ICloudinaryService cloudinaryService;
        private readonly IEmailService emailService;

        public UserService(UserManager<AraboonUser> userManager, RoleManager<AraboonRole> roleManager,
                           IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork, AraboonDbContext context,
                           ICloudinaryService cloudinaryService, IEmailService emailService)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.httpContextAccessor = httpContextAccessor;
            this.unitOfWork = unitOfWork;
            this.context = context;
            this.cloudinaryService = cloudinaryService;
            this.emailService = emailService;
        }

        public async Task<string> ChangeBioAsync(string bio)
        {
            var userId = unitOfWork.UserRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";
            user.Bio = bio;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return "AnErrorOccurredWhileChangingTheBio";
            return "BioChangedSuccessfully";
        }

        public async Task<string> ChangeEmailAsync(string email)
        {
            var userId = unitOfWork.UserRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";
            try
            {
                var httpRequest = httpContextAccessor.HttpContext.Request;
                var token = await userManager.GenerateChangeEmailTokenAsync(user, email);
                var link = $"{httpRequest.Scheme}://{httpRequest.Host}/{Router.UserRouting.ChangeEmailConfirmation}?userId={userId}&email={email}&token={Uri.EscapeDataString(token)}";
                var send = await emailService.SendAuthenticationsEmailAsync(email, link, "Change Your Email", $"{user.FirstName} {user.LastName}");
                if (send.Equals("Failed"))
                    return "AnErrorOccurredWhileSendingTheChangeEmailPleaseTryAgain";
                return "ChangeEmailHasBeenSent";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileSendingTheChangeEmailPleaseTryAgain";
            }

        }
        public async Task<string> ChangeEmailConfirmationAsync(string userId, string email, string token)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user is null)
                    return "UserNotFound";
                var result = await userManager.ChangeEmailAsync(user, email, token);
                if (!result.Succeeded)
                    return "InvalidOrExpiredToken";
                return "EmailChangedSuccessfully";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredDuringTheChangeEmailProcess";
            }
        }
        public async Task<string> ChangeNameAsync(string firstName, string lastName)
        {
            var userId = unitOfWork.UserRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";
            user.FirstName = firstName;
            user.LastName = lastName;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return "AnErrorOccurredWhileChangingTheName";
            return "NameChangedSuccessfully";
        }
        public async Task<string> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            var userId = unitOfWork.UserRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
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
            var userId = unitOfWork.UserRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
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
            var userId = unitOfWork.UserRepository.ExtractUserIdFromToken();
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
                    UserName = user.UserName,
                    Email = string.IsNullOrWhiteSpace(userId) ? null : user.Id.ToString().Equals(userId) ? user.Email : null,
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
                                f => f.UserID.Equals(user.Id) && f.Manga.CategoryMangas.Any(c => c.Category.CategoryNameEn.Equals(category.CategoryNameEn))
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
        public async Task<string> UploadCoverImageAsync(IFormFile image, IFormFile croppedImage)
        {
            var userId = unitOfWork.UserRepository.ExtractUserIdFromToken();
            if (string.IsNullOrWhiteSpace(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var originalUrl = user.CoverImage?.OriginalImage;
                    if (string.IsNullOrWhiteSpace(originalUrl))
                    {
                        var cloudinaryResult = await cloudinaryService.DeleteFileAsync(originalUrl);
                        if (cloudinaryResult.Equals("FailedToDeleteImageFromCloudinary"))
                            return "FailedToDeleteOldOriginalImageFromCloudinary";
                    }
                    var croppedUrl = user.CoverImage?.CroppedImage;
                    if (string.IsNullOrWhiteSpace(croppedUrl))
                    {
                        var cloudinaryResult = await cloudinaryService.DeleteFileAsync(croppedUrl);
                        if (cloudinaryResult.Equals("FailedToDeleteImageFromCloudinary"))
                            return "FailedToDeleteOldCroppedImageFromCloudinary";
                    }
                    var guidPart = Guid.NewGuid().ToString("N").Substring(0, 12);
                    var datePart = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                    var originalId = $"original-{guidPart}-{datePart}";
                    var croppedId = $"cropped-{guidPart}-{datePart}";
                    using (var stream = image.OpenReadStream())
                    {
                        var (imageName, folderName) = (originalId, $"ARABOON/Accounts/{user.Id}/CoverImage");
                        var url = await cloudinaryService.UploadFileAsync(stream, folderName, imageName);
                        user.CoverImage.OriginalImage = url;
                    }
                    using (var stream = croppedImage.OpenReadStream())
                    {
                        var (imageName, folderName) = (croppedId, $"ARABOON/Accounts/{user.Id}/CoverImage");
                        var url = await cloudinaryService.UploadFileAsync(stream, folderName, imageName);
                        user.CoverImage.CroppedImage = url;
                    }
                    var result = await userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return "AnErrorOccurredWhileEditingCoverImage";
                    }
                    await transaction.CommitAsync();
                    return "TheCoverImageHasBeenChangedSuccessfully";
                }
                catch (Exception exp)
                {
                    if (transaction.GetDbTransaction().Connection is not null)
                        await transaction.RollbackAsync();
                    return "AnErrorOccurredWhileProcessingYourCoverImageModificationRequest";
                }
            }
        }
        public async Task<string> UploadProfileImageAsync(IFormFile image, CropData cropData)
        {
            var userId = unitOfWork.UserRepository.ExtractUserIdFromToken();
            if (String.IsNullOrWhiteSpace(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var originalUrl = user.ProfileImage?.OriginalImage;
                    if (string.IsNullOrWhiteSpace(originalUrl))
                    {
                        var cloudinaryResult = await cloudinaryService.DeleteFileAsync(originalUrl);
                        if (cloudinaryResult.Equals("FailedToDeleteImageFromCloudinary"))
                            return "FailedToDeleteOldImageFromCloudinary";
                    }
                    var guidPart = Guid.NewGuid().ToString("N").Substring(0, 12);
                    var datePart = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                    var id = $"{guidPart}-{datePart}";
                    using (var stream = image.OpenReadStream())
                    {
                        var (imageName, folderName) = (id, $"ARABOON/Accounts/{user.Id}/ImageProfile");
                        var url = await cloudinaryService.UploadFileAsync(stream, folderName, imageName);
                        user.ProfileImage.OriginalImage = url;
                    }
                    user.ProfileImage.X = cropData.Position.X;
                    user.ProfileImage.Y = cropData.Position.Y;
                    user.ProfileImage.Scale = cropData.Scale;
                    user.ProfileImage.Rotate = cropData.Rotate;
                    var result = await userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return "AnErrorOccurredWhileEditingImageData";
                    }
                    await transaction.CommitAsync();
                    return "TheImageHasBeenChangedSuccessfully";
                }
                catch (Exception exp)
                {
                    if (transaction.GetDbTransaction().Connection is not null)
                        await transaction.RollbackAsync();
                    return "AnErrorOccurredWhileProcessingYourProfileImageModificationRequest";
                }
            }
        }
        public async Task<string> DeleteProfileImage()
        {
            var userId = unitOfWork.UserRepository.ExtractUserIdFromToken();
            if (String.IsNullOrWhiteSpace(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";
            var url = user.ProfileImage?.OriginalImage;
            if (string.IsNullOrWhiteSpace(url))
                return "ThereIsNoImageToDelete";
            try
            {
                var cloudinaryResult = cloudinaryService.DeleteFileAsync(url);
                if (cloudinaryResult.Equals("FailedToDeleteImageFromCloudinary"))
                    return "FailedToDeleteImageFromCloudinary";
                user.ProfileImage.OriginalImage = null;
                var result = await userManager.UpdateAsync(user);
                return result.Succeeded ? "ImageHasBeenSuccessfullyDeleted" : "AnErrorOccurredWhileSaving";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileDeletingTheImage";
            }
        }

        public async Task<string> DeleteCoverImage()
        {
            var userId = unitOfWork.UserRepository.ExtractUserIdFromToken();
            if (String.IsNullOrWhiteSpace(userId))
                return "UserNotFound";
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return "UserNotFound";
            var originalUrl = user.CoverImage?.OriginalImage;
            var croppedUrl = user.CoverImage?.CroppedImage;
            if (string.IsNullOrWhiteSpace(originalUrl) || string.IsNullOrWhiteSpace(croppedUrl))
                return "ThereIsNoImageToDelete";
            try
            {
                var originalCloudinary = await cloudinaryService.DeleteFileAsync(originalUrl);
                if (originalCloudinary.Equals("FailedToDeleteImageFromCloudinary"))
                    return "FailedToDeleteOriginalImageFromCloudinary";
                var croppedCloudinary = await cloudinaryService.DeleteFileAsync(croppedUrl);
                if (croppedCloudinary.Equals("FailedToDeleteImageFromCloudinary"))
                    return "FailedToDeleteCroppedImageFromCloudinary";
                user.CoverImage.OriginalImage = null;
                user.CoverImage.CroppedImage = null;
                var result = await userManager.UpdateAsync(user);
                return result.Succeeded ? "ImageHasBeenSuccessfullyDeleted" : "AnErrorOccurredWhileSaving";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileDeletingTheImage";
            }
        }
    }
}
