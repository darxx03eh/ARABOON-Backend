using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Commands.Models
{
    public class ActivateOrDeActivateMangaCommand : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public ActivateOrDeActivateMangaCommand(int id) => Id = id;
    }
}
