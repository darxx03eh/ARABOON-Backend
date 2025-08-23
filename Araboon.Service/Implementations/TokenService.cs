using Araboon.Data.Entities.Identity;
using Araboon.Service.Interfaces;
using Araboon.Data.Helpers;
using Araboon.Data.Response.Authentications;
using Araboon.Infrastructure.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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
        public async Task<SignInResponse> GenerateAccessTokenAsync(AraboonUser user)
        {
            var (jwtToken, accessToken) = await GenerateJwtTokenAsync(user);
            var refreshToken = await GenerateRefreshToken(user.UserName);
            var userRefreshToken = new UserRefreshToken()
            {
                AddedTime = DateTime.UtcNow,
                ExpirydDate = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpireDate),
                IsUsed = true,
                IsRevoked = false,
                JwtID = jwtToken.Id,
                Token = accessToken,
                UserID = user.Id,
                RefreshToken = refreshToken.Token
            };
            await refreshTokenRepository.AddAsync(userRefreshToken);
            return new SignInResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        public async Task<(JwtSecurityToken, string)> GenerateJwtTokenAsync(AraboonUser user)
        {
            var userClaims = await GenerateUserClaimsAsync(user);
            var jwtToken = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: userClaims,
                expires: DateTime.UtcNow.AddDays(jwtSettings.AccessTokenExpireDate),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey))
                    , SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return (jwtToken, accessToken);
        }
        private async Task<List<Claim>> GenerateUserClaimsAsync(AraboonUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>()
            {
                new Claim(nameof(UserClaimModel.UserName), user.UserName),
                new Claim(nameof(UserClaimModel.Email), user.Email),
                new Claim(nameof(UserClaimModel.FirstName), user.FirstName),
                new Claim(nameof(UserClaimModel.LastName), user.LastName),
                new Claim(nameof(UserClaimModel.ID), user.Id.ToString())
            };
            claims.AddRange(roles.Select(role => new Claim(nameof(UserClaimModel.Role), role)));
            return claims;
        }
        private async Task<RefreshToken> GenerateRefreshToken(string username)
            => new RefreshToken()
            {
                UserName = username,
                ExpireAt = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpireDate),
                Token = await GenerateRandomRefreshToken()
            };
        public async Task<string> GenerateRandomRefreshToken()
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
