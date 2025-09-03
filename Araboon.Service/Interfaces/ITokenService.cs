using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Authentications;
using System.IdentityModel.Tokens.Jwt;

namespace Araboon.Service.Interfaces
{
    public interface ITokenService
    {
        public Task<(SignInResponse, string)> GenerateAccessTokenAsync(AraboonUser user);
        public Task<JwtSecurityToken> ReadJwtTokenAsync(string token);
        public Task<(JwtSecurityToken, string)> GenerateJwtTokenAsync(AraboonUser user);
        public Task<string> GenerateRandomToken();
        public Task<(string, string)> GenerateRefreshToken(AraboonUser user);
    }
}
