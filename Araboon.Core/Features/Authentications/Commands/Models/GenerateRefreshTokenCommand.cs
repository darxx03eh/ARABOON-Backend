using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Authentications.Commands.Models
{
    public class GenerateRefreshTokenCommand : IRequest<ApiResponse>
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
