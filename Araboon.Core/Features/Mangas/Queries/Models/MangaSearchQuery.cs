using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Queries.Models
{
    public class MangaSearchQuery : IRequest<ApiResponse>
    {
        public string? Search { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
