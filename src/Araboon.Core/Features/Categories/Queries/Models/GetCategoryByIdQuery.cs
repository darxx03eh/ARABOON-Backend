using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Categories.Queries.Models
{
    public class GetCategoryByIdQuery : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public GetCategoryByIdQuery(int id) => Id = id;
    }
}
