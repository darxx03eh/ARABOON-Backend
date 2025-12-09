using Araboon.Data.Response.Users.Queries;
using Araboon.Data.Wrappers;
using Microsoft.AspNetCore.Http;

namespace Araboon.Service.Interfaces
{
    public interface IUserService
    {
        public Task<(string, UserProfileResponse?)> GetUserProfileAsync(string username);
        public Task<string> ChangePasswordAsync(string currentPassword, string newPassword);
        public Task<string> ChangeUserNameAsync(string username);
        public Task<string> UploadProfileImageAsync(IFormFile image, CropData cropData);
        public Task<string> UploadCoverImageAsync(IFormFile image, IFormFile croppedImage);
        public Task<string> ChangeEmailAsync(string email);
        public Task<string> ChangeEmailConfirmationAsync(string id, string email, string token);
        public Task<string> ChangeBioAsync(string bio);
        public Task<string> ChangeNameAsync(string firstName, string lastName);
        public Task<string> DeleteProfileImageAsync();
        public Task<string> DeleteCoverImageAsync();
        public Task<string> ChangeCroppedDataAsync(CropData cropData);
        public Task<string> ChangeCroppedCoverImageAsync(IFormFile image);
        public Task<(string, PaginatedResult<UserManagementResponse>?, UsersMetaDataResponse?)> GetUsersForDashboardAsync(
            int pageNumber, int pageSize, string? search = null
        );
        public Task<string> ActivateUserToggleAsync(int id);
        public Task<string> ChangeRoleToggleAsync(int id);
    }
}
