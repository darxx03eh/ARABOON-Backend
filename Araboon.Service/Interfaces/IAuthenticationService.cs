using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Authentications;

namespace Araboon.Service.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<String> RegistrationUserAsync(AraboonUser user, String password);
        public Task<(SignInResponse?, String)> SignInAsync(String username, String password);
        public Task<String> ConfirmationEmailAsync(String email, String token);
        public Task<String> SendConfirmationEmailAsync(String email);
        public Task<String> SendForgetPasswordAsync(String email);
        public Task<(String, String?)> ForgetPasswordConfirmationAsync(String email, String code);
        public Task<String> ResetPasswordAsync(String email, String password, String token);
        public Task<(SignInResponse, String)> GenerateRefreshTokenAsync(String accessToken, String refreshToken);
        public Task<String> RevokeRefreshTokenAsync(String refreshToken);
        public Task<String> ValidateAccessToken(String token);
    }
}
