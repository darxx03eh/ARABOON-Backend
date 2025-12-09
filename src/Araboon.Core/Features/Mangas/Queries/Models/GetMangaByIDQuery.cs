using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Queries.Models
{
    public class GetMangaByIDQuery : IRequest<ApiResponse>
    {
        public int ID { get; set; }
        public GetMangaByIDQuery(int id)
            => ID = id;
    }
}
