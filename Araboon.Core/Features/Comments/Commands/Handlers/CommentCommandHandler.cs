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
        , IRequestHandler<UpdateCommentCommand, ApiResponse>
        , IRequestHandler<AddLikeToCommentCommand, ApiResponse>
        , IRequestHandler<DeleteLikeFromCommentCommand, ApiResponse>
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
            var (result, comment) = await commentService.AddCommentAsync(request.Content.Trim(), request.MangaId);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "AnErrorOccurredWhileCommenting" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileCommenting]),
                "CommentCompletedSuccessfully" => 
                Success(comment, message:stringLocalizer[SharedTranslationKeys.CommentCompletedSuccessfully]),
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
                Forbidden(stringLocalizer[SharedTranslationKeys.YouAreNotTheOwnerOfThisCommentOrYouAreNotTheAdmin]),
                "AnErrorOccurredWhileDeletingTheComment" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheComment]),
                "TheCommentHasBeenSuccessfullyDeleted" =>
                Success(null, message: stringLocalizer[SharedTranslationKeys.TheCommentHasBeenSuccessfullyDeleted]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheComment])
            };
        }

        public async Task<ApiResponse> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var (result, content, since) = await commentService.UpdateCommentAsync(request.Content.Trim(), request.Id);
            return result switch
            {
                "CommentNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CommentNotFound]),
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "YouAreNotTheOwnerOfThisCommentOrYouAreNotTheAdmin" =>
                BadRequest(stringLocalizer[SharedTranslationKeys.YouAreNotTheOwnerOfThisCommentOrYouAreNotTheAdmin]),
                "AnErrorOccurredWhileDeletingTheComment" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileUpdatingTheComment]),
                "TheCommentHasBeenSuccessfullyUpdated" => 
                Success(new
                {
                    Content = content,
                    Since = since,
                }, message: stringLocalizer[SharedTranslationKeys.TheCommentHasBeenSuccessfullyUpdated]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileUpdatingTheComment])
            };
        }

        public async Task<ApiResponse> Handle(AddLikeToCommentCommand request, CancellationToken cancellationToken)
        {
            var result = await commentService.AddLikeAsync(request.Id);
            return result switch
            {
                "CommentNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CommentNotFound]),
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "YouAreAlreadyAddedLikeToThisComment" => Conflict(stringLocalizer[SharedTranslationKeys.YouAreAlreadyAddedLikeToThisComment]),
                "TheLikeProcessForThisCommentFailed" => InternalServerError(stringLocalizer[SharedTranslationKeys.TheLikeProcessForThisCommentFailed]),
                "AnErrorOccurredWhileAddingALikeToTheComment" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingALikeToTheComment]),
                "TheLikeHasBeenAddedToTheCommentSuccessfully" => 
                Success(null, message: stringLocalizer[SharedTranslationKeys.TheLikeHasBeenAddedToTheCommentSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingALikeToTheComment])
            };
        }

        public async Task<ApiResponse> Handle(DeleteLikeFromCommentCommand request, CancellationToken cancellationToken)
        {
            var result = await commentService.DeleteLikeAsync(request.Id);
            return result switch
            {
                "CommentNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CommentNotFound]),
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "YouAreNotLikedThisComment" => BadRequest(stringLocalizer[SharedTranslationKeys.YouAreNotLikedThisComment]),
                "AnErrorOccurredWhileRemovingALikeFromTheComment" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileRemovingALikeFromTheComment]),
                "TheLikeHasBeenDeletedFromTheCommentSuccessfully" => 
                Success(null, message: stringLocalizer[SharedTranslationKeys.TheLikeHasBeenDeletedFromTheCommentSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileRemovingALikeFromTheComment])
            };
        }
    }
}
