using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Mangas.Queries.Models
{
    public class GetMangaByCategoryNameQuery : IRequest<ApiResponse>
    {
        public String CategoryName { get; set; }
        public Int32 PageNumber { get; set; }
        public Int32 PageSize { get; set; }
        public GetMangaByCategoryNameQuery(String category)
            => CategoryName = category;
    }
}
