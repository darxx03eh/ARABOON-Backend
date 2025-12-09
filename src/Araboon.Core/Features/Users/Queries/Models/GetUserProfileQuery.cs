using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Users.Queries.Models
{
    public class GetUserProfileQuery : IRequest<ApiResponse>
    {
        public string UserName { get; set; }
        public GetUserProfileQuery(string username)
            => UserName = username;
    }
}
