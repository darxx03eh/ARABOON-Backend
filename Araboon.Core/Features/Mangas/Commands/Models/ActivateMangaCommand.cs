using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Commands.Models
{
    public class ActivateMangaCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public ActivateMangaCommand(int id) => Id = id;
    }
}
