using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Queries.Models
{
    public class GetMangaCommentsQuery : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public GetMangaCommentsQuery(int id)
            => Id = id;
    }
}
