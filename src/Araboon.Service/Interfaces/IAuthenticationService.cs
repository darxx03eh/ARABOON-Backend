using Araboon.Data.Entities.Identity;
using Araboon.Data.Response.Authentications;

namespace Araboon.Service.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<string> RegistrationUserAsync(AraboonUser user, string password);
        public Task<(SignInResponse?, string, string?)> SignInAsync(string username, string password);
        public Task<string> ConfirmationEmailAsync(string email, string token);
        public Task<string> SendConfirmationEmailAsync(string email);
        public Task<string> SendForgetPasswordAsync(string email);
        public Task<(string, string?)> ForgetPasswordConfirmationAsync(string email, string code);
        public Task<string> ResetPasswordAsync(string email, string password, string token);
        public Task<(SignInResponse, string)> GenerateRefreshTokenAsync(string refresh);
        public Task<string> LogOutAsync(string refresh);
        public Task<string> ValidateAccessToken(string token);
    }
}
