using Araboon.Core.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Araboon.Core.Features.Swipers.Commands.Models
{
    public class AddNewSwiperCommand : IRequest<ApiResponse>
    {
        public IFormFile Image { get; set; }
        public string? Note { get; set; }
    }
}
