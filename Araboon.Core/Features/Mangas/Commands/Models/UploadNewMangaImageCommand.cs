using Araboon.Core.Bases;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Araboon.Core.Features.Mangas.Commands.Models
{
    public class UploadNewMangaImageCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public IFormFile Image { get; set; }
    }
}
