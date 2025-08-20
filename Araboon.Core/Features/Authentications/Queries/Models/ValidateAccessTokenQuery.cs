using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Queries.Models
{
    public class ValidateAccessTokenQuery : IRequest<ApiResponse>
    {
        public String AccessToken { get; set; }
        public ValidateAccessTokenQuery(String token)
            => AccessToken = token;
    }
}
