using Araboon.Core.Bases;
using Araboon.Data.Enums;
using MediatR;

namespace Araboon.Core.Features.Mangas.Queries.Models
{
    public class GetMangaByStatusQuery : IRequest<ApiResponse>
    {
        public string Status { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public MangaOrderingEnum OrderBy { get; set; }
        public string? Filter { get; set; }
    }
}
