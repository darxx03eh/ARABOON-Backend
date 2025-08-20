using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Queries.Models
{
    public class GetPaginatedHottestMangaQuery : IRequest<ApiResponse>
    {
        public Int32 PageNumber { get; set; }
        public Int32 PageSize { get; set; }
    }
}
