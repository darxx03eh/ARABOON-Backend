using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Queries.Models
{
    public class GetPaginatedHottestMangaQuery : IRequest<ApiResponse>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
