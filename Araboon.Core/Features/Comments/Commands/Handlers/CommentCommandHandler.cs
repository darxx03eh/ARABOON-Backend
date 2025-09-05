using Araboon.Core.Bases;
using Araboon.Core.Features.Comments.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Comments.Commands.Handlers
{
    public class CommentCommandHandler : ApiResponseHandler
        , IRequestHandler<AddCommentCommand, ApiResponse>
        , IRequestHandler<DeleteCommentCommand, ApiResponse>
    {
        private readonly ICommentService commentService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public CommentCommandHandler(ICommentService commentService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.commentService = commentService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var result = await commentService.AddCommentAsync(request.Content.Trim(), request.MangaId);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "AnErrorOccurredWhileCommenting" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileCommenting]),
                "CommentCompletedSuccessfully" => Success(null, message:stringLocalizer[SharedTranslationKeys.CommentCompletedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileCommenting])
            };
        }

        public async Task<ApiResponse> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var result = await commentService.DeleteCommentAsync(request.Id);
            return result switch
            {
                "CommentNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CommentNotFound]),
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "YouAreNotTheOwnerOfThisCommentOrYouAreNotTheAdmin" =>
                BadRequest(stringLocalizer[SharedTranslationKeys.YouAreNotTheOwnerOfThisCommentOrYouAreNotTheAdmin]),
                "AnErrorOccurredWhileDeletingTheComment" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheComment]),
                "TheCommentHasBeenSuccessfullyDeleted" =>
                Success(null, message: stringLocalizer[SharedTranslationKeys.TheCommentHasBeenSuccessfullyDeleted]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheComment])
            };
        }
    }
}
