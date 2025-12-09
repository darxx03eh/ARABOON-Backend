using Araboon.Core.Bases;
using Araboon.Data.DTOs.Mangas;
using MediatR;

namespace Araboon.Core.Features.Mangas.Commands.Models
{
    public class UpdateMangaCommand : UpdateMangaInfoDTO, IRequest<ApiResponse>
    {
        public int MangaId { get; set; }
        public UpdateMangaCommand(int mangaId) => MangaId = mangaId;
    }
}
