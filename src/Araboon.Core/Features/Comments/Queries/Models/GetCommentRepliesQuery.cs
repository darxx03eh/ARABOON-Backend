using Araboon.Core.Bases;
using MediatR;

namespace Araboon.Core.Features.Comments.Queries.Models
{
    public class GetCommentRepliesQuery : IRequest<ApiResponse>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public GetCommentRepliesQuery(int id)
            => Id = id;
    }
}
