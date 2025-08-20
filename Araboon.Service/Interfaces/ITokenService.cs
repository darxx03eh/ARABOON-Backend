using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Authentications;
using System.IdentityModel.Tokens.Jwt;

namespace Araboon.Service.Interfaces
{
    public interface ITokenService
    {
        public Task<SignInResponse> GenerateAccessTokenAsync(AraboonUser user);
        public Task<JwtSecurityToken> ReadJwtTokenAsync(String token);
        public Task<(JwtSecurityToken, String)> GenerateJwtTokenAsync(AraboonUser user);
        public Task<String> GenerateRandomRefreshToken();
    }
}
