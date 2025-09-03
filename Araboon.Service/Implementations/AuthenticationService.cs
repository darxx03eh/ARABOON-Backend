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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        public AuthenticationService(AraboonDbContext context, UserManager<AraboonUser> userManager, RoleManager<AraboonRole> roleManager,
                                    IHttpContextAccessor httpContextAccessor, IEmailService emailService,
                                    ITokenService tokenService, SignInManager<AraboonUser> signInManager,
                                    IRefreshTokenRepository refreshTokenRepository, JwtSettings jwtSettings,
                                    IGenericRepository<AraboonUser> genericRepository, IAvatarService avatarService,
                                    ICloudinaryService cloudinaryService)
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
        }
        public async Task<string> RegistrationUserAsync(AraboonUser user, string password)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var result = await userManager.CreateAsync(user, password);
                    if (!result.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return "ErrorDuringAccountCreationProcess";
                    }
                    var role = await roleManager.RoleExistsAsync(Roles.User);
                    if (!role)
                    {
                        await transaction.RollbackAsync();
                        return "RoleNotExist";
                    }
                    var roleResult = await userManager.AddToRoleAsync(user, Roles.User);
                    if (!roleResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return "ErrorWhileAddingRole";
                    }
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var httpRequest = httpContextAccessor.HttpContext.Request;
                    var link = $"{httpRequest.Scheme}://{httpRequest.Host}/{Router.AuthenticationRouting.EmailConfirmation}?email={user.Email}&token={Uri.EscapeDataString(token)}";
                    var sendEmail = await emailService.SendAuthenticationsEmailAsync(user.Email, link, "Verification Email", $"{user.FirstName} {user.LastName}");
                    if (sendEmail.Equals("Failed"))
                    {
                        await transaction.RollbackAsync();
                        return "AnErrorOccurredWhileSendingTheConfirmationEmailPleaseTryAgain";
                    }
                    var avatarLink = $"https://ui-avatars.com/api/?name={user.FirstName}+{user.LastName}&background=random&color=fff&format=png";
                    var stream = await avatarService.DownloadImageAsStreamAsync(avatarLink);
                    var (imageName, folderName) = ("defaultImage", $"ARABOON/Accounts/{user.Id}/ImageProfile");
                    var imageUrl = await cloudinaryService.UploadFileAsync(stream, folderName, imageName);
                    user.ProfileImage = new ProfileImage() { OriginalImage = imageUrl };
                    user.CoverImage = new CoverImage() { UserID = user.Id };
                    var updateResult = await userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return "AnErrorOccurredWhileAddingTheImage";
                    }
                    await transaction.CommitAsync();
                    return "TheAccountHasBeenCreated";

                }
                catch (Exception exp)
                {
                    if (transaction.GetDbTransaction().Connection is not null)
                        await transaction.RollbackAsync();
                    return "AnErrorOccurredDuringTheRegistrationProcess";
                }
            }
            throw new Exception();
        }

        public async Task<(SignInResponse?, string, string?)> SignInAsync(string username, string password)
        {
            try
            {
                var user = await userManager.FindByNameAsync(username);
                if (user is null)
                    return (null, "PasswordOrUserNameWrnog", null);
                if (user.LockoutEnd is not null && DateTime.UtcNow <= user.LockoutEnd)
                    return (null, "YourAccountWasLockedDueToSuspiciousActivityPleaseTryAgainLaterorContactSupport", null);
                var result = await signInManager.CheckPasswordSignInAsync(user, password, true);
                if (!result.Succeeded)
                {
                    if (!user.EmailConfirmed)
                        return (null, "EmailNotConfirmed", null);
                    return (null, "PasswordOrUserNameWrnog", null);
                }
                if (!user.IsActive)
                    return (null, "YourAccountIsInactivePleaseCheckWithTechnicalSupport", null);
                var (token, refresh) = await tokenService.GenerateAccessTokenAsync(user);
                if (token is null)
                    return (null, "AnErrorOccurredWhileGeneratingTheToken", null);
                return (token, "DataVerifiedAndLogin", refresh);
            }
            catch (Exception exp)
            {
                return (null, "AnErrorOccurredDuringTheLoginProcess", null);
            }
        }

        public async Task<string> ConfirmationEmailAsync(string email, string token)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user is null)
                    return "UserNotFound";
                var result = await userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                    return "InvalidOrExpiredToken";
                return "EmailConfirmedSuccessfully";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredDuringTheEmailConfirmationProcess";
            }
        }
        public async Task<(string, string?)> ForgetPasswordConfirmationAsync(string email, string code)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user is null)
                    return ("UserNotFound", null);
                if (user.CodeExpiryDate <= DateTime.UtcNow)
                    return ("TheCodeHasExpired", null);
                if (!code.Equals(user.Code))
                    return ("TheCodeEnteredIsIncorrect", null);
                (user.Code, user.CodeExpiryDate) = (null, null);
                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return ("AnErrorOccurredWhileDeletingTheCode", null);
                var token = await tokenService.GenerateRandomToken();
                user.ForgetPasswordToken = token;
                var tokenResult = await userManager.UpdateAsync(user);
                if (!tokenResult.Succeeded)
                    return ("AnErrorOccurredWhileSavingThePasswordresetToken", null);
                return ("TheCodeHasbeenVerified", token);
            }
            catch (Exception exp)
            {
                return ("ThereWasAProblemValidatingTheCode", null);
            }
        }

        public async Task<(SignInResponse?, string)> GenerateRefreshTokenAsync(string refresh)
        {
            var jwtToken = await tokenService.ReadJwtTokenAsync(refresh);
            var (message, expireDate) = await ValidateDetails(jwtToken, refresh);
            switch (message)
            {
                case "AlgorithmIsWrong":
                    return (null, "ErrorInTheEncryptionAlgorithmUsed");
                case "RefreshTokenIsNotFound":
                    return (null, "RefreshTokenIsNotFound");
                case "RefreshTokenIsExpire":
                    return (null, "RefreshTokenHasExpire");
            }
            var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type.Equals(nameof(UserClaimModel.ID)))?.Value;
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return (null, "UserNotFound");
            var result = await GenerateRefreshTokenAsync(user);
            if (result is null)
                return (null, "AnErrorOccurredDuringTheTokenGenerationProcess");
            return (result, "AccessTokenRegenerated");
        }
        private async Task<(string, DateTime?)> ValidateDetails(JwtSecurityToken jwtToken, string refresh)
        {
            if (jwtToken is null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
                return ("AlgorithmIsWrong", null);
            var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type.Equals(nameof(UserClaimModel.ID)))?.Value;
            var jti = jwtToken.Claims.FirstOrDefault(claim => claim.Type.Equals(nameof(UserClaimModel.Jti)))?.Value;
            var refreshToken = await refreshTokenRepository.GetTableNoTracking().Where(r => r.Jti.Equals(jti)).FirstOrDefaultAsync();
            if (refreshToken is null)
                return ("RefreshTokenIsNotFound", null);
            if (refreshToken.ExpirydDate < DateTime.UtcNow)
            {
                refreshToken.IsRevoked = true;
                refreshToken.IsUsed = false;
                await refreshTokenRepository.UpdateAsync(refreshToken);
                return ("RefreshTokenIsExpire", null);
            }
            var expireDate = refreshToken.ExpirydDate;
            return (userId, expireDate);
        }
        private async Task<SignInResponse> GenerateRefreshTokenAsync(AraboonUser user)
        {
            var (jwtSecurityToken, responseToken) = await tokenService.GenerateJwtTokenAsync(user);
            return new SignInResponse() { Access = responseToken };
        }
        public async Task<string> ResetPasswordAsync(string email, string password, string token)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
                return "UserNotFound";
            var test = user.ForgetPasswordToken;
            if (!token.Equals(user.ForgetPasswordToken))
                return "InvalidPasswordResetToken";
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var removeResult = await userManager.RemovePasswordAsync(user);
                    if (!removeResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return "AnErrorOccurredWhileDeletingTheOldPassword";
                    }
                    var addResult = await userManager.AddPasswordAsync(user, password);
                    if (!addResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return "AnErrorOccurredWhileAddingTheNewPassword";
                    }
                    user.ForgetPasswordToken = null;
                    var deleteResult = await userManager.UpdateAsync(user);
                    if (!deleteResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return "ThereWasAProblemDeletingThePasswordResetToken";
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

        public async Task<string> LogOutAsync(string refresh)
        {
            var jwtToken = await tokenService.ReadJwtTokenAsync(refresh);
            var jti = jwtToken.Claims.FirstOrDefault(claim => claim.Type.Equals(nameof(UserClaimModel.Jti)))?.Value;

            var token = await refreshTokenRepository.GetTableNoTracking().FirstOrDefaultAsync(r => r.Jti.Equals(jti));
            if (token is null || token.IsRevoked || token.ExpirydDate <= DateTime.UtcNow)
                return "InvalidRefreshToken";
            token.IsUsed = false;
            token.IsRevoked = true;
            token.ExpirydDate = DateTime.UtcNow;
            await refreshTokenRepository.UpdateAsync(token);
            return "LogOutSuccessfully";
        }

        public async Task<string> SendConfirmationEmailAsync(string username)
        {
            try
            {
                var user = await userManager.FindByNameAsync(username);
                if (user is null)
                    return "UserNotFound";
                if (user.EmailConfirmed)
                    return "YourAccountHasAlreadyBeenConfirmed";
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var httpRequest = httpContextAccessor.HttpContext.Request;
                var link = $"{httpRequest.Scheme}://{httpRequest.Host}/{Router.AuthenticationRouting.EmailConfirmation}?email={user.Email}&token={Uri.EscapeDataString(token)}";
                var send = await emailService.SendAuthenticationsEmailAsync(user.Email, link, "Verification Email", $"{user.FirstName} {user.LastName}");
                if (send.Equals("Failed"))
                    return "AnErrorOccurredWhileSendingTheConfirmationEmailPleaseTryAgain";
                return "EmailConfirmationEmailHasBeenSent";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileSendingTheConfirmationEmailPleaseTryAgain";
            }
        }

        public async Task<string> SendForgetPasswordAsync(string email)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user is null)
                    return "UserNotFound";
                var guid = Guid.NewGuid().ToByteArray();
                var code = (int)(BitConverter.ToUInt32(guid, 0) % 900000) + 100000;
                user.Code = code.ToString();
                user.CodeExpiryDate = DateTime.UtcNow.AddMinutes(10);
                var codeResult = await userManager.UpdateAsync(user);
                if (!codeResult.Succeeded)
                    return "AnErrorOccurredWhileSavingTheCode";
                var emailResult = await emailService.SendAuthenticationsEmailAsync(email, code.ToString(), "Forget Password", $"{user.FirstName} {user.LastName}");
                if (emailResult.Equals("Failed"))
                    return "AnErrorOccurredWhileSendingTheForgetPasswordEmailPleaseTryAgain";
                return "ForgetPasswordEmailHasBeenSent";
            }
            catch (Exception exp)
            {
                return "AnErrorOccurredWhileSendingTheForgetPasswordEmailPleaseTryAgain";
            }
        }
        public async Task<string> ValidateAccessToken(string token)
        {

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
                    return "InvalidTokenFormat";
                return "ValidToken";
            }
            catch (SecurityTokenExpiredException exp)
            {
                return "TokenExpired";
            }
            catch (Exception exp)
            {
                return "InvalidToken";
            }
        }
    }
}
