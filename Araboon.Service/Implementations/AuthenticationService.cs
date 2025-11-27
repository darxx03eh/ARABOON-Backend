using Araboon.Data.Entities;
using Araboon.Data.Entities.Identity;
using Araboon.Data.Helpers;
using Araboon.Data.Response.Authentications;
using Araboon.Data.Routing;
using Araboon.Infrastructure.Data;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Araboon.Service.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AraboonDbContext context;
        private readonly UserManager<AraboonUser> userManager;
        private readonly RoleManager<AraboonRole> roleManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IEmailService emailService;
        private readonly ITokenService tokenService;
        private readonly SignInManager<AraboonUser> signInManager;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly JwtSettings jwtSettings;
        private readonly IGenericRepository<AraboonUser> genericRepository;
        private readonly IAvatarService avatarService;
        private readonly ICloudinaryService cloudinaryService;
        private readonly ILogger<AuthenticationService> logger;

        public AuthenticationService(AraboonDbContext context, UserManager<AraboonUser> userManager, RoleManager<AraboonRole> roleManager,
                                    IHttpContextAccessor httpContextAccessor, IEmailService emailService,
                                    ITokenService tokenService, SignInManager<AraboonUser> signInManager,
                                    IRefreshTokenRepository refreshTokenRepository, JwtSettings jwtSettings,
                                    IGenericRepository<AraboonUser> genericRepository, IAvatarService avatarService,
                                    ICloudinaryService cloudinaryService,
                                    ILogger<AuthenticationService> logger)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.httpContextAccessor = httpContextAccessor;
            this.emailService = emailService;
            this.tokenService = tokenService;
            this.signInManager = signInManager;
            this.refreshTokenRepository = refreshTokenRepository;
            this.jwtSettings = jwtSettings;
            this.genericRepository = genericRepository;
            this.avatarService = avatarService;
            this.cloudinaryService = cloudinaryService;
            this.logger = logger;
        }

        public async Task<string> RegistrationUserAsync(AraboonUser user, string password)
        {
            logger.LogInformation("Registration started - بدء عملية تسجيل مستخدم جديد | Email: {Email}", user?.Email);

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    logger.LogInformation("Creating user - إنشاء المستخدم | Email: {Email}", user.Email);
                    var result = await userManager.CreateAsync(user, password);
                    if (!result.Succeeded)
                    {
                        logger.LogWarning("Error during account creation process - حدث خطأ أثناء إنشاء الحساب | Errors: {Errors}",
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                        await transaction.RollbackAsync();
                        return "ErrorDuringAccountCreationProcess";
                    }

                    logger.LogInformation("Checking if role exists - التحقق من وجود الدور USER");
                    var role = await roleManager.RoleExistsAsync(Roles.User);
                    if (!role)
                    {
                        logger.LogWarning("Role User not exist - دور User غير موجود في النظام");
                        await transaction.RollbackAsync();
                        return "RoleNotExist";
                    }

                    logger.LogInformation("Adding user to role - إضافة المستخدم إلى الدور | Role: {Role}", Roles.User);
                    var roleResult = await userManager.AddToRoleAsync(user, Roles.User);
                    if (!roleResult.Succeeded)
                    {
                        logger.LogWarning("Error while adding role - خطأ أثناء إضافة الدور للمستخدم | Errors: {Errors}",
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                        await transaction.RollbackAsync();
                        return "ErrorWhileAddingRole";
                    }

                    logger.LogInformation("Generating email confirmation token - إنشاء رمز تأكيد البريد الإلكتروني | Email: {Email}", user.Email);
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var httpRequest = httpContextAccessor.HttpContext.Request;
                    var link = $"{httpRequest.Scheme}://{httpRequest.Host}/{Router.AuthenticationRouting.EmailConfirmation}?email={user.Email}&token={Uri.EscapeDataString(token)}";

                    logger.LogInformation("Sending verification email - إرسال بريد التحقق | Email: {Email}", user.Email);
                    var sendEmail = await emailService.SendAuthenticationsEmailAsync(user.Email, link, "Verification Email", $"{user.FirstName} {user.LastName}");
                    if (sendEmail.Equals("Failed"))
                    {
                        logger.LogWarning("Failed to send confirmation email - فشل في إرسال بريد التحقق | Email: {Email}", user.Email);
                        await transaction.RollbackAsync();
                        return "AnErrorOccurredWhileSendingTheConfirmationEmailPleaseTryAgain";
                    }

                    logger.LogInformation("Generating default avatar - إنشاء صورة افتراضية للمستخدم | Email: {Email}", user.Email);
                    var avatarLink = $"https://ui-avatars.com/api/?name={user.FirstName}+{user.LastName}&background=random&color=fff&format=png";
                    var stream = await avatarService.DownloadImageAsStreamAsync(avatarLink);

                    logger.LogInformation("Uploading avatar to cloudinary - رفع صورة البروفايل إلى كلاوديناري | UserId: {UserId}", user.Id);
                    var (imageName, folderName) = ("defaultImage", $"ARABOON/Accounts/{user.Id}/ImageProfile");
                    var imageUrl = await cloudinaryService.UploadFileAsync(stream, folderName, imageName);

                    user.ProfileImage = new ProfileImage() { OriginalImage = imageUrl };
                    user.CoverImage = new CoverImage() { UserID = user.Id };

                    logger.LogInformation("Updating user with profile and cover image - تحديث بيانات المستخدم بالصورة الشخصية وصورة الغلاف | UserId: {UserId}", user.Id);
                    var updateResult = await userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        logger.LogWarning("Error while adding image - خطأ أثناء إضافة الصورة للمستخدم | Errors: {Errors}",
                            string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                        await transaction.RollbackAsync();
                        return "AnErrorOccurredWhileAddingTheImage";
                    }

                    await transaction.CommitAsync();
                    logger.LogInformation("Registration completed successfully - تم إنشاء الحساب بنجاح | Email: {Email}", user.Email);
                    return "TheAccountHasBeenCreated";
                }
                catch (Exception exp)
                {
                    logger.LogError(exp, "An error occurred during the registration process - حدث خطأ أثناء عملية التسجيل | Email: {Email}", user?.Email);
                    if (transaction.GetDbTransaction().Connection is not null)
                        await transaction.RollbackAsync();
                    return "AnErrorOccurredDuringTheRegistrationProcess";
                }
            }
        }

        public async Task<(SignInResponse?, string, string?)> SignInAsync(string username, string password)
        {
            logger.LogInformation("Sign in attempt - محاولة تسجيل الدخول | Username: {Username}", username);

            try
            {
                var user = await userManager.FindByNameAsync(username);
                if (user is null)
                {
                    logger.LogWarning("User not found during sign in - لم يتم العثور على المستخدم أثناء تسجيل الدخول | Username: {Username}", username);
                    return (null, "PasswordOrUserNameWrnog", null);
                }

                if (user.LockoutEnd is not null && DateTime.UtcNow <= user.LockoutEnd)
                {
                    logger.LogWarning("Account locked due to suspicious activity - الحساب مقفل بسبب نشاط مريب | Username: {Username}", username);
                    return (null, "YourAccountWasLockedDueToSuspiciousActivityPleaseTryAgainLaterorContactSupport", null);
                }

                logger.LogInformation("Checking password - التحقق من كلمة المرور | Username: {Username}", username);
                var result = await signInManager.CheckPasswordSignInAsync(user, password, true);
                if (!result.Succeeded)
                {
                    if (!user.EmailConfirmed)
                    {
                        logger.LogWarning("Email not confirmed during login - البريد الإلكتروني غير مؤكد أثناء تسجيل الدخول | Username: {Username}", username);
                        return (null, "EmailNotConfirmed", null);
                    }

                    logger.LogWarning("Wrong username or password - اسم المستخدم أو كلمة المرور غير صحيحة | Username: {Username}", username);
                    return (null, "PasswordOrUserNameWrnog", null);
                }

                if (!user.IsActive)
                {
                    logger.LogWarning("Inactive account login attempt - محاولة تسجيل دخول لحساب غير مفعل | Username: {Username}", username);
                    return (null, "YourAccountIsInactivePleaseCheckWithTechnicalSupport", null);
                }

                logger.LogInformation("Generating access and refresh tokens - إنشاء توكن الوصول وتوكن التحديث | UserId: {UserId}", user.Id);
                var (token, refresh) = await tokenService.GenerateAccessTokenAsync(user);
                if (token is null)
                {
                    logger.LogError("Error while generating token - حدث خطأ أثناء إنشاء التوكن | UserId: {UserId}", user.Id);
                    return (null, "AnErrorOccurredWhileGeneratingTheToken", null);
                }

                user.LastLogin = DateTime.UtcNow;
                logger.LogInformation("Updating last login - تحديث وقت آخر تسجيل دخول | UserId: {UserId}", user.Id);
                var lastLoginResult = await userManager.UpdateAsync(user);
                if (!lastLoginResult.Succeeded)
                {
                    logger.LogError("Error while saving last login - حدث خطأ أثناء حفظ وقت آخر تسجيل دخول | UserId: {UserId}", user.Id);
                    return (null, "AnErrorOccurredWhileSavingTheLastLogin", null);
                }

                logger.LogInformation("Sign in successful - تم تسجيل الدخول بنجاح | Username: {Username}", username);
                return (token, "DataVerifiedAndLogin", refresh);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "An error occurred during the login process - حدث خطأ أثناء عملية تسجيل الدخول | Username: {Username}", username);
                return (null, "AnErrorOccurredDuringTheLoginProcess", null);
            }
        }

        public async Task<string> ConfirmationEmailAsync(string email, string token)
        {
            logger.LogInformation("Email confirmation attempt - محاولة تأكيد البريد الإلكتروني | Email: {Email}", email);

            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user is null)
                {
                    logger.LogWarning("User not found during email confirmation - المستخدم غير موجود أثناء تأكيد البريد | Email: {Email}", email);
                    return "UserNotFound";
                }

                logger.LogInformation("Confirming email token - تأكيد رمز البريد الإلكتروني | Email: {Email}", email);
                var result = await userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    logger.LogWarning("Invalid or expired token - رمز تأكيد غير صالح أو منتهي | Email: {Email}", email);
                    return "InvalidOrExpiredToken";
                }

                logger.LogInformation("Email confirmed successfully - تم تأكيد البريد الإلكتروني بنجاح | Email: {Email}", email);
                return "EmailConfirmedSuccessfully";
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "An error occurred during the email confirmation process - حدث خطأ أثناء عملية تأكيد البريد الإلكتروني | Email: {Email}", email);
                return "AnErrorOccurredDuringTheEmailConfirmationProcess";
            }
        }

        public async Task<(string, string?)> ForgetPasswordConfirmationAsync(string email, string code)
        {
            logger.LogInformation("Forget password code confirmation attempt - محاولة تأكيد كود نسيان كلمة المرور | Email: {Email}", email);

            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user is null)
                {
                    logger.LogWarning("User not found during forget password confirmation - المستخدم غير موجود أثناء تأكيد نسيان كلمة المرور | Email: {Email}", email);
                    return ("UserNotFound", null);
                }

                if (user.CodeExpiryDate <= DateTime.UtcNow)
                {
                    logger.LogWarning("Code has expired - الكود منتهي الصلاحية | Email: {Email}", email);
                    return ("TheCodeHasExpired", null);
                }

                if (!code.Equals(user.Code))
                {
                    logger.LogWarning("Incorrect code entered - الكود المدخل غير صحيح | Email: {Email}", email);
                    return ("TheCodeEnteredIsIncorrect", null);
                }

                logger.LogInformation("Clearing code and expiry date - حذف الكود وتاريخ الانتهاء | Email: {Email}", email);
                (user.Code, user.CodeExpiryDate) = (null, null);
                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    logger.LogError("Error while deleting the code - خطأ أثناء حذف الكود | Email: {Email}", email);
                    return ("AnErrorOccurredWhileDeletingTheCode", null);
                }

                logger.LogInformation("Generating random token for password reset - إنشاء توكن عشوائي لإعادة تعيين كلمة المرور | Email: {Email}", email);
                var token = await tokenService.GenerateRandomToken();
                user.ForgetPasswordToken = token;
                var tokenResult = await userManager.UpdateAsync(user);
                if (!tokenResult.Succeeded)
                {
                    logger.LogError("Error while saving the password reset token - خطأ أثناء حفظ توكن إعادة التعيين | Email: {Email}", email);
                    return ("AnErrorOccurredWhileSavingThePasswordresetToken", null);
                }

                logger.LogInformation("Forget password code verified successfully - تم التحقق من كود نسيان كلمة المرور بنجاح | Email: {Email}", email);
                return ("TheCodeHasbeenVerified", token);
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "There was a problem validating the code - حدثت مشكلة أثناء التحقق من الكود | Email: {Email}", email);
                return ("ThereWasAProblemValidatingTheCode", null);
            }
        }

        public async Task<(SignInResponse?, string)> GenerateRefreshTokenAsync(string refresh)
        {
            logger.LogInformation("Generate refresh token called - تم استدعاء توليد توكن تحديث");

            var jwtToken = await tokenService.ReadJwtTokenAsync(refresh);
            var (message, expireDate) = await ValidateDetails(jwtToken, refresh);

            switch (message)
            {
                case "AlgorithmIsWrong":
                    logger.LogWarning("Encryption algorithm is wrong - خوارزمية التشفير غير صحيحة");
                    return (null, "ErrorInTheEncryptionAlgorithmUsed");

                case "RefreshTokenIsNotFound":
                    logger.LogWarning("Refresh token not found - لم يتم العثور على توكن التحديث");
                    return (null, "RefreshTokenIsNotFound");

                case "RefreshTokenIsExpire":
                    logger.LogWarning("Refresh token has expired - توكن التحديث منتهي الصلاحية");
                    return (null, "RefreshTokenHasExpire");

                case "RefreshTokenWasRevoked":
                    logger.LogWarning("Refresh token was revoked - تم إلغاء توكن التحديث");
                    return (null, "RefreshTokenWasRevoked");

                case "RefreshTokenNotUsedAnyMore":
                    logger.LogWarning("Refresh token not used anymore - لم يعد توكن التحديث مستخدمًا");
                    return (null, "RefreshTokenNotUsedAnyMore");
            }

            var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type.Equals(nameof(UserClaimModel.ID)))?.Value;
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("User not found during refresh token generation - المستخدم غير موجود أثناء إنشاء توكن جديد | UserId: {UserId}", userId);
                return (null, "UserNotFound");
            }

            if (!user.IsActive)
            {
                logger.LogWarning("Account is disabled - الحساب معطل ولا يمكن إنشاء توكن وصول جديد | UserId: {UserId}", userId);
                return (null, "TheAccountIsDisabledAndCannotBeRegeneratedWithAnAccessToken");
            }

            user.LastLogin = DateTime.UtcNow;
            logger.LogInformation("Updating last login before generating new token - تحديث آخر تسجيل دخول قبل إنشاء توكن جديد | UserId: {UserId}", userId);
            var loginResult = await userManager.UpdateAsync(user);
            if (!loginResult.Succeeded)
            {
                logger.LogError("An error occurred during the token generation process (updating last login) - حدث خطأ أثناء إنشاء التوكن عند تحديث آخر تسجيل دخول | UserId: {UserId}", userId);
                return (null, "AnErrorOccurredDuringTheTokenGenerationProcess");
            }

            var result = await GenerateRefreshTokenAsync(user);
            if (result is null)
            {
                logger.LogError("An error occurred during the token generation process (creating new token) - حدث خطأ أثناء إنشاء التوكن الجديد | UserId: {UserId}", userId);
                return (null, "AnErrorOccurredDuringTheTokenGenerationProcess");
            }

            logger.LogInformation("Access token regenerated successfully - تم إعادة إنشاء توكن الوصول بنجاح | UserId: {UserId}", userId);
            return (result, "AccessTokenRegenerated");
        }

        private async Task<(string, DateTime?)> ValidateDetails(JwtSecurityToken jwtToken, string refresh)
        {
            logger.LogInformation("Validating refresh token details - التحقق من تفاصيل توكن التحديث");

            if (jwtToken is null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                logger.LogWarning("Algorithm is wrong for JWT token - خوارزمية JWT غير صحيحة");
                return ("AlgorithmIsWrong", null);
            }

            var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type.Equals(nameof(UserClaimModel.ID)))?.Value;
            var jti = jwtToken.Claims.FirstOrDefault(claim => claim.Type.Equals(nameof(UserClaimModel.Jti)))?.Value;

            logger.LogInformation("Fetching refresh token from repository - جلب توكن التحديث من المستودع | Jti: {Jti}", jti);
            var refreshToken = await refreshTokenRepository.GetTableNoTracking().Where(r => r.Jti.Equals(jti)).FirstOrDefaultAsync();
            if (refreshToken is null)
            {
                logger.LogWarning("Refresh token not found in database - لم يتم العثور على توكن التحديث في قاعدة البيانات | Jti: {Jti}", jti);
                return ("RefreshTokenIsNotFound", null);
            }

            if (refreshToken.IsRevoked)
            {
                logger.LogWarning("Refresh token was revoked - تم إلغاء توكن التحديث | Jti: {Jti}", jti);
                return ("RefreshTokenWasRevoked", null);
            }

            if (!refreshToken.IsUsed)
            {
                logger.LogWarning("Refresh token not used anymore - توكن التحديث لم يعد مستخدمًا | Jti: {Jti}", jti);
                return ("RefreshTokenNotUsedAnyMore", null);
            }

            if (refreshToken.ExpirydDate < DateTime.UtcNow)
            {
                logger.LogWarning("Refresh token has expired, marking it revoked - توكن التحديث منتهي، سيتم وضعه كملغي | Jti: {Jti}", jti);
                refreshToken.IsRevoked = true;
                refreshToken.IsUsed = false;
                await refreshTokenRepository.UpdateAsync(refreshToken);
                return ("RefreshTokenIsExpire", null);
            }

            var expireDate = refreshToken.ExpirydDate;
            logger.LogInformation("Refresh token is valid - توكن التحديث صالح | UserId: {UserId}, Expire: {Expire}", userId, expireDate);
            return (userId, expireDate);
        }

        private async Task<SignInResponse> GenerateRefreshTokenAsync(AraboonUser user)
        {
            logger.LogInformation("Generating JWT access token - إنشاء JWT جديد لتوكن الوصول | UserId: {UserId}", user.Id);
            var (jwtSecurityToken, responseToken) = await tokenService.GenerateJwtTokenAsync(user);
            return new SignInResponse() { Access = responseToken };
        }

        public async Task<string> ResetPasswordAsync(string email, string password, string token)
        {
            logger.LogInformation("Reset password attempt - محاولة إعادة تعيين كلمة المرور | Email: {Email}", email);

            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                logger.LogWarning("User not found during reset password - المستخدم غير موجود أثناء إعادة تعيين كلمة المرور | Email: {Email}", email);
                return "UserNotFound";
            }

            var test = user.ForgetPasswordToken;
            if (!token.Equals(user.ForgetPasswordToken))
            {
                logger.LogWarning("Invalid password reset token - توكن إعادة تعيين كلمة المرور غير صالح | Email: {Email}", email);
                return "InvalidPasswordResetToken";
            }

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    logger.LogInformation("Removing old password - حذف كلمة المرور القديمة | Email: {Email}", email);
                    var removeResult = await userManager.RemovePasswordAsync(user);
                    if (!removeResult.Succeeded)
                    {
                        logger.LogError("An error occurred while deleting the old password - حدث خطأ أثناء حذف كلمة المرور القديمة | Email: {Email}", email);
                        await transaction.RollbackAsync();
                        return "AnErrorOccurredWhileDeletingTheOldPassword";
                    }

                    logger.LogInformation("Adding new password - إضافة كلمة مرور جديدة | Email: {Email}", email);
                    var addResult = await userManager.AddPasswordAsync(user, password);
                    if (!addResult.Succeeded)
                    {
                        logger.LogError("An error occurred while adding the new password - حدث خطأ أثناء إضافة كلمة المرور الجديدة | Email: {Email}", email);
                        await transaction.RollbackAsync();
                        return "AnErrorOccurredWhileAddingTheNewPassword";
                    }

                    user.ForgetPasswordToken = null;
                    logger.LogInformation("Clearing forget password token - مسح توكن نسيان كلمة المرور | Email: {Email}", email);
                    var deleteResult = await userManager.UpdateAsync(user);
                    if (!deleteResult.Succeeded)
                    {
                        logger.LogError("There was a problem deleting the password reset token - حدثت مشكلة أثناء حذف توكن إعادة تعيين كلمة المرور | Email: {Email}", email);
                        await transaction.RollbackAsync();
                        return "ThereWasAProblemDeletingThePasswordResetToken";
                    }

                    await transaction.CommitAsync();
                    logger.LogInformation("Password changed successfully - تم تغيير كلمة المرور بنجاح | Email: {Email}", email);
                    return "PasswordChangedSuccessfully";
                }
                catch (Exception exp)
                {
                    logger.LogError(exp, "An error occurred while changing the password - حدث خطأ أثناء تغيير كلمة المرور | Email: {Email}", email);
                    if (transaction.GetDbTransaction().Connection is not null)
                        await transaction.RollbackAsync();
                    return "AnErrorOccurredWhileChangingThePassword";
                }
            }
        }

        public async Task<string> LogOutAsync(string refresh)
        {
            logger.LogInformation("Logout attempt - محاولة تسجيل خروج");

            var jwtToken = await tokenService.ReadJwtTokenAsync(refresh);
            var jti = jwtToken.Claims.FirstOrDefault(claim => claim.Type.Equals(nameof(UserClaimModel.Jti)))?.Value;

            logger.LogInformation("Fetching refresh token for logout - جلب توكن التحديث لتسجيل الخروج | Jti: {Jti}", jti);
            var token = await refreshTokenRepository.GetTableNoTracking().FirstOrDefaultAsync(r => r.Jti.Equals(jti));
            if (token is null || token.IsRevoked || token.ExpirydDate <= DateTime.UtcNow)
            {
                logger.LogWarning("Invalid refresh token while logout - توكن تحديث غير صالح أثناء تسجيل الخروج | Jti: {Jti}", jti);
                return "InvalidRefreshToken";
            }

            token.IsUsed = false;
            token.IsRevoked = true;
            token.ExpirydDate = DateTime.UtcNow;

            logger.LogInformation("Updating refresh token as revoked - تحديث توكن التحديث ليصبح ملغى | Jti: {Jti}", jti);
            await refreshTokenRepository.UpdateAsync(token);

            logger.LogInformation("Log out successfully - تم تسجيل الخروج بنجاح");
            return "LogOutSuccessfully";
        }

        public async Task<string> SendConfirmationEmailAsync(string username)
        {
            logger.LogInformation("Send confirmation email attempt - محاولة إرسال بريد تأكيد | Username: {Username}", username);

            try
            {
                var user = await userManager.FindByNameAsync(username);
                if (user is null)
                {
                    logger.LogWarning("User not found while sending confirmation email - المستخدم غير موجود أثناء إرسال بريد التأكيد | Username: {Username}", username);
                    return "UserNotFound";
                }

                if (user.EmailConfirmed)
                {
                    logger.LogWarning("Account already confirmed - الحساب مؤكد مسبقًا | Username: {Username}", username);
                    return "YourAccountHasAlreadyBeenConfirmed";
                }

                logger.LogInformation("Generating confirmation email token - إنشاء رمز تأكيد البريد الإلكتروني | Username: {Username}", username);
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var httpRequest = httpContextAccessor.HttpContext.Request;
                var link = $"{httpRequest.Scheme}://{httpRequest.Host}/{Router.AuthenticationRouting.EmailConfirmation}?email={user.Email}&token={Uri.EscapeDataString(token)}";

                logger.LogInformation("Sending confirmation email - إرسال بريد التأكيد | Email: {Email}", user.Email);
                var send = await emailService.SendAuthenticationsEmailAsync(user.Email, link, "Verification Email", $"{user.FirstName} {user.LastName}");
                if (send.Equals("Failed"))
                {
                    logger.LogError("An error occurred while sending the confirmation email - حدث خطأ أثناء إرسال بريد التأكيد | Email: {Email}", user.Email);
                    return "AnErrorOccurredWhileSendingTheConfirmationEmailPleaseTryAgain";
                }

                logger.LogInformation("Email confirmation email has been sent - تم إرسال بريد تأكيد الحساب | Email: {Email}", user.Email);
                return "EmailConfirmationEmailHasBeenSent";
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "An error occurred while sending the confirmation email - حدث خطأ أثناء إرسال بريد تأكيد الحساب | Username: {Username}", username);
                return "AnErrorOccurredWhileSendingTheConfirmationEmailPleaseTryAgain";
            }
        }

        public async Task<string> SendForgetPasswordAsync(string email)
        {
            logger.LogInformation("Send forget password email attempt - محاولة إرسال بريد نسيان كلمة المرور | Email: {Email}", email);

            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user is null)
                {
                    logger.LogWarning("User not found while sending forget password email - المستخدم غير موجود أثناء إرسال بريد نسيان كلمة المرور | Email: {Email}", email);
                    return "UserNotFound";
                }

                var guid = Guid.NewGuid().ToByteArray();
                var code = (int)(BitConverter.ToUInt32(guid, 0) % 900000) + 100000;

                logger.LogInformation("Generating forget password code - إنشاء كود نسيان كلمة المرور | Email: {Email}, Code: {Code}", email, code);
                user.Code = code.ToString();
                user.CodeExpiryDate = DateTime.UtcNow.AddMinutes(10);

                var codeResult = await userManager.UpdateAsync(user);
                if (!codeResult.Succeeded)
                {
                    logger.LogError("An error occurred while saving the code - حدث خطأ أثناء حفظ الكود | Email: {Email}", email);
                    return "AnErrorOccurredWhileSavingTheCode";
                }

                logger.LogInformation("Sending forget password email - إرسال بريد نسيان كلمة المرور | Email: {Email}", email);
                var emailResult = await emailService.SendAuthenticationsEmailAsync(email, code.ToString(), "Forget Password", $"{user.FirstName} {user.LastName}");
                if (emailResult.Equals("Failed"))
                {
                    logger.LogError("An error occurred while sending the forget password email - حدث خطأ أثناء إرسال بريد نسيان كلمة المرور | Email: {Email}", email);
                    return "AnErrorOccurredWhileSendingTheForgetPasswordEmailPleaseTryAgain";
                }

                logger.LogInformation("Forget password email has been sent - تم إرسال بريد نسيان كلمة المرور | Email: {Email}", email);
                return "ForgetPasswordEmailHasBeenSent";
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "An error occurred while sending the forget password email - حدث خطأ أثناء إرسال بريد نسيان كلمة المرور | Email: {Email}", email);
                return "AnErrorOccurredWhileSendingTheForgetPasswordEmailPleaseTryAgain";
            }
        }

        public async Task<string> ValidateAccessToken(string token)
        {
            logger.LogInformation("Validate access token called - تم استدعاء التحقق من توكن الوصول");

            var handler = new JwtSecurityTokenHandler();
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = jwtSettings.ValidateIssuer,
                ValidIssuer = jwtSettings.Issuer,
                ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
                ValidateAudience = true,
                ValidAudiences = new[] { jwtSettings.Audience },
                ValidateLifetime = jwtSettings.ValidateLifetime,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                var principal = handler.ValidateToken(token, parameters, out SecurityToken validatedToken);
                if (validatedToken is not JwtSecurityToken jwtToken)
                {
                    logger.LogWarning("Invalid token format - تنسيق التوكن غير صالح");
                    return "InvalidTokenFormat";
                }

                logger.LogInformation("Token is valid - التوكن صالح");
                return "ValidToken";
            }
            catch (SecurityTokenExpiredException exp)
            {
                logger.LogWarning(exp, "Token expired - التوكن منتهي الصلاحية");
                return "TokenExpired";
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Invalid token - التوكن غير صالح");
                return "InvalidToken";
            }
        }
    }
}