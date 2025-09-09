using Araboon.Core.Bases;
using Araboon.Core.Features.Replies.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Replies.Commands.Handlers
{
    public class ReplyCommandHandler : ApiResponseHandler
        , IRequestHandler<AddReplyToCommentCommand, ApiResponse>
        , IRequestHandler<DeleteReplyCommand, ApiResponse>
    {
        private readonly IReplyService replyService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public ReplyCommandHandler(IReplyService replyService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.replyService = replyService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(AddReplyToCommentCommand request, CancellationToken cancellationToken)
        {
            var(result, replies) = await replyService.AddReplyAsync(request.Content, request.Id);
            return result switch
            {
                "CommentNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CommentNotFound]),
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "AnErrorOccurredWhileRepling" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileRepling]),
                "ReplyCompletedSuccessfully" => 
                Success(replies, message: stringLocalizer[SharedTranslationKeys.ReplyCompletedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileRepling])
            };
        }

        public async Task<ApiResponse> Handle(DeleteReplyCommand request, CancellationToken cancellationToken)
        {
            var result = await replyService.DeleteReplyAsync(request.Id);
            return result switch
            {
                "ReplyNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.ReplyNotFound]),
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "YouAreNotTheOwnerOfThisReplyOrYouAreNotTheAdmin" => 
                Forbidden(stringLocalizer[SharedTranslationKeys.YouAreNotTheOwnerOfThisReplyOrYouAreNotTheAdmin]),
                "AnErrorOccurredWhileDeletingTheReply" => 
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheReply]),
                "TheReplyHasBeenSuccessfullyDeleted" => Success(null, message: stringLocalizer[SharedTranslationKeys.TheReplyHasBeenSuccessfullyDeleted]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheReply])
            };
        }
    }
}
