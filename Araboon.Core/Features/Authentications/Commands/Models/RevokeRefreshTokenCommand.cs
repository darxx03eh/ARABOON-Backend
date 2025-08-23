using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Commands.Models
{
    public class RevokeRefreshTokenCommand : IRequest<ApiResponse>
    {
        public string RefreshToken { get; set; }
    }
}
