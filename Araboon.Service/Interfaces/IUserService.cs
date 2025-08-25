using Araboon.Data.Response.Users.Queries;

namespace Araboon.Service.Interfaces
{
    public interface IUserService
    {
        public Task<(string, UserProfileResponse?)> GetUserProfileAsync(string username);
        public Task<string> ChangePasswordAsync(string currentPassword, string newPassword);
        public Task<string> ChangeUserNameAsync(string username);
    }
}
