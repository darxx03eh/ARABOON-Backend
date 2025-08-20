using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Queries.Models
{
    public class GetMangaByIDQuery : IRequest<ApiResponse>
    {
        public Int32 ID { get; set; }
        public GetMangaByIDQuery(Int32 id)
            => ID = id;
    }
}
