using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Queries.Models
{
    public class GetCommentsCountQuery : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public GetCommentsCountQuery(int id) => Id = id;
    }
}
