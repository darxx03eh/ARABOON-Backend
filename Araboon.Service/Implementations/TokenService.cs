using Araboon.Data.Entities.Identity;
using Araboon.Data.Helpers;
using Araboon.Data.Response.Authentications;
using Araboon.Data.Response.Users.Queries;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Araboon.Service.Implementations
{
    public class TokenService(UserManager<AraboonUser> userManager
                            , JwtSettings jwtSettings
                            , IRefreshTokenRepository refreshTokenRepository) : ITokenService
    {
        private readonly UserManager<AraboonUser> userManager = userManager;
        private readonly JwtSettings jwtSettings = jwtSettings;
        private readonly IRefreshTokenRepository refreshTokenRepository = refreshTokenRepository;

        public async Task<(SignInResponse, string)> GenerateAccessTokenAsync(AraboonUser user)
        {
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
            return (new SignInResponse() { Access = accessToken }, refreshToken);
        }

        public async Task<(JwtSecurityToken, string)> GenerateJwtTokenAsync(AraboonUser user)
        {
            var userClaims = await GenerateUserClaimsAsync(user);
            var jwtToken = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: userClaims,
                expires: DateTime.UtcNow.AddHours(jwtSettings.AccessTokenExpireDate),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey))
                    , SecurityAlgorithms.HmacSha256));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return (jwtToken, accessToken);
        }

        private async Task<List<Claim>> GenerateUserClaimsAsync(AraboonUser user)
        {
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
            claims.AddRange(roles.Select(role => new Claim(nameof(UserClaimModel.Role), role)));
            return claims;
        }

        public async Task<(string, string)> GenerateRefreshToken(AraboonUser user)
        {
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
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey))
                    , SecurityAlgorithms.HmacSha256));
            var refreshToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return (refreshToken, jti);
        }

        public async Task<string> GenerateRandomToken()
        {
            var random = new byte[32];
            var randomGenerate = RandomNumberGenerator.Create();
            randomGenerate.GetBytes(random);
            return Convert.ToBase64String(random);
        }

        public async Task<JwtSecurityToken> ReadJwtTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            var handler = new JwtSecurityTokenHandler();
            var response = handler.ReadJwtToken(token);
            return response;
        }
    }
}