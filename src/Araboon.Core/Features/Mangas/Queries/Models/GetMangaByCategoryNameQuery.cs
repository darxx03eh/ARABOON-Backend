using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Queries.Models
{
    public class GetMangaByCategoryNameQuery : IRequest<ApiResponse>
    {
        public string CategoryName { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public GetMangaByCategoryNameQuery(string category)
            => CategoryName = category;
    }
}
