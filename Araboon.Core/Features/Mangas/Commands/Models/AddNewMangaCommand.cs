using Araboon.Core.Bases;
using Araboon.Data.DTOs.Mangas;
using MediatR;

namespace Araboon.Core.Features.Mangas.Commands.Models
{
    public class AddNewMangaCommand : MangaInfoDTO, IRequest<ApiResponse>
    {
    }
}
