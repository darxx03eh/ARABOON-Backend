using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Commands.Models
{
    public class DeActivateMangaCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public DeActivateMangaCommand(int id) => Id = id;
    }
}
