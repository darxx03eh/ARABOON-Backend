using Araboon.Core.Bases;
using Araboon.Core.Features.Comments.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Service.Implementations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Comments.Queries.Handlers
{
    public class CommentQueryHandler : ApiResponseHandler
        , IRequestHandler<GetCommentRepliesQuery, ApiResponse>
    {
        private readonly ICommentService commentService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public CommentQueryHandler(ICommentService commentService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.commentService = commentService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(GetCommentRepliesQuery request, CancellationToken cancellationToken)
        {
            var (result, replies) = await commentService.GetCommentRepliesAsync(request.Id, request.PageNumber, request.PageSize);
            return result switch
            {
                "CommentNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CommentNotFound]),
                "RepliesNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.RepliesNotFound]),
                "RepliesFound" => Success(replies, message: stringLocalizer[SharedTranslationKeys.RepliesFound]),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.RepliesNotFound])
            };
        }
    }
}
