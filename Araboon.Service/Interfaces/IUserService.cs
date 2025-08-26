using Araboon.Data.Response.Users.Queries;
using Microsoft.AspNetCore.Http;

namespace Araboon.Service.Interfaces
{
    public interface IUserService
    {
        public Task<(string, UserProfileResponse?)> GetUserProfileAsync(string username);
        public Task<string> ChangePasswordAsync(string currentPassword, string newPassword);
        public Task<string> ChangeUserNameAsync(string username);
        public Task<string> UploadProfileImageAsync(IFormFile image, CropData cropData);
    }
}
