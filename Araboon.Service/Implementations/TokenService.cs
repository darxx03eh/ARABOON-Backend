using Araboon.Data.Entities.Identity;
using Araboon.Data.Helpers;
using Araboon.Data.Response.Authentications;
using Araboon.Data.Response.Users.Queries;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Araboon.Service.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<AraboonUser> userManager;
        private readonly JwtSettings jwtSettings;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly ILogger<TokenService> logger;

        public TokenService(
            UserManager<AraboonUser> userManager,
            JwtSettings jwtSettings,
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<TokenService> logger)
        {
            this.userManager = userManager;
            this.jwtSettings = jwtSettings;
            this.refreshTokenRepository = refreshTokenRepository;
            this.logger = logger;
        }

        public async Task<(SignInResponse, string)> GenerateAccessTokenAsync(AraboonUser user)
        {
            logger.LogInformation("Generating access token - إنشاء توكن الوصول | UserId: {Id}", user.Id);

            var (jwtToken, accessToken) = await GenerateJwtTokenAsync(user);
            var (refreshToken, jti) = await GenerateRefreshToken(user);

            var userRefreshToken = new UserRefreshToken()
            {
                AddedTime = DateTime.UtcNow,
                ExpirydDate = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpireDate),
                IsUsed = true,
                IsRevoked = false,
                Token = refreshToken,
                Jti = jti,
                UserID = user.Id,
            };

            await refreshTokenRepository.AddAsync(userRefreshToken);

            logger.LogInformation("Access token generated successfully - تم إنشاء توكن الوصول بنجاح | UserId: {Id}", user.Id);

            return (new SignInResponse() { Access = accessToken }, refreshToken);
        }

        public async Task<(JwtSecurityToken, string)> GenerateJwtTokenAsync(AraboonUser user)
        {
            logger.LogInformation("Generating JWT token - إنشاء توكن JWT | UserId: {Id}", user.Id);

            var userClaims = await GenerateUserClaimsAsync(user);

            var jwtToken = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: userClaims,
                expires: DateTime.UtcNow.AddHours(jwtSettings.AccessTokenExpireDate),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
                    SecurityAlgorithms.HmacSha256));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            logger.LogInformation("JWT token generated successfully - تم إنشاء توكن JWT بنجاح | UserId: {Id}", user.Id);

            return (jwtToken, accessToken);
        }

        private async Task<List<Claim>> GenerateUserClaimsAsync(AraboonUser user)
        {
            logger.LogInformation("Generating user claims - إنشاء Claims المستخدم | UserId: {Id}", user.Id);

            var roles = await userManager.GetRolesAsync(user);

            var profileImage = new ProfileImage()
            {
                OriginalImage = user.ProfileImage?.OriginalImage,
                CropData = new CropData()
                {
                    Position = new Position()
                    {
                        X = user.ProfileImage.X,
                        Y = user.ProfileImage.Y
                    },
                    Scale = user.ProfileImage.Scale,
                    Rotate = user.ProfileImage.Rotate
                }
            };

            var claims = new List<Claim>()
            {
                new Claim(nameof(UserClaimModel.Type), "access"),
                new Claim(nameof(UserClaimModel.UserName), user.UserName),
                new Claim(nameof(UserClaimModel.Email), user.Email),
                new Claim(nameof(UserClaimModel.FirstName), user.FirstName),
                new Claim(nameof(UserClaimModel.LastName), user.LastName),
                new Claim(nameof(UserClaimModel.ID), user.Id.ToString()),
                new Claim(nameof(UserClaimModel.ProfileImage), JsonConvert.SerializeObject(profileImage))
            };

            claims.AddRange(roles.Select(role =>
                new Claim(nameof(UserClaimModel.Role), role)));

            logger.LogInformation("User claims generated successfully - تم إنشاء Claims المستخدم بنجاح | UserId: {Id}", user.Id);

            return claims;
        }

        public async Task<(string, string)> GenerateRefreshToken(AraboonUser user)
        {
            logger.LogInformation("Generating refresh token - إنشاء توكن التحديث | UserId: {Id}", user.Id);

            var jti = Guid.NewGuid().ToString();
            var claims = new List<Claim>()
            {
                new Claim(nameof(UserClaimModel.Jti), jti),
                new Claim(nameof(UserClaimModel.ID), user.Id.ToString()),
                new Claim(nameof(UserClaimModel.Type), "refresh")
            };

            var jwtToken = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpireDate),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
                    SecurityAlgorithms.HmacSha256));

            var refreshToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            logger.LogInformation("Refresh token generated successfully - تم إنشاء توكن التحديث بنجاح | UserId: {Id}", user.Id);

            return (refreshToken, jti);
        }

        public async Task<string> GenerateRandomToken()
        {
            logger.LogInformation("Generating random token - إنشاء توكن عشوائي");

            var random = new byte[32];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(random);

            return Convert.ToBase64String(random);
        }

        public async Task<JwtSecurityToken> ReadJwtTokenAsync(string token)
        {
            logger.LogInformation("Reading JWT token - قراءة توكن JWT");

            if (string.IsNullOrEmpty(token))
            {
                logger.LogError("Token cannot be null - التوكن لا يمكن أن يكون فارغاً");
                throw new ArgumentNullException(nameof(token));
            }

            var handler = new JwtSecurityTokenHandler();
            var response = handler.ReadJwtToken(token);

            logger.LogInformation("JWT token read successfully - تم قراءة التوكن بنجاح");

            return response;
        }
    }
}