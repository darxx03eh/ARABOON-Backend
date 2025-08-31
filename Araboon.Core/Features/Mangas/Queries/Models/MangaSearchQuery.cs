using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Queries.Models
{
    public class MangaSearchQuery : IRequest<ApiResponse>
    {
        public string Search { get; set; }
        public MangaSearchQuery(string search)
            => Search = search;
    }
}
