using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Commands.Models
{
    public class DeleteMangaImageCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public DeleteMangaImageCommand(int id) => Id = id;
    }
}
