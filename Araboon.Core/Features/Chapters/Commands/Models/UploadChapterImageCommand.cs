using Araboon.Core.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Araboon.Core.Features.Chapters.Commands.Models
{
    public class UploadChapterImageCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public IFormFile Image { get; set; }
    }
}
