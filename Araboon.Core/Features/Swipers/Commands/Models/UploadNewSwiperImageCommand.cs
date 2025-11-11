using Araboon.Core.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Araboon.Core.Features.Swipers.Commands.Models
{
    public class UploadNewSwiperImageCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public IFormFile Image {  get; set; }
    }
}
