using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Queries.Models
{
    public class ValidateAccessTokenQuery : IRequest<ApiResponse>
    {
        public string AccessToken { get; set; }
        public ValidateAccessTokenQuery(string token)
            => AccessToken = token;
    }
}
