using Araboon.Data.Response.Users.Queries;

namespace Araboon.Service.Interfaces
{
    public interface IUserService
    {
        public Task<(string, UserProfileResponse?)> GetUserProfileAsync(string username);
    }
}
