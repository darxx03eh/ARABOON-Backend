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
        , IRequestHandler<UpdateReplyCommand, ApiResponse>
        , IRequestHandler<AddLikeToReplyCommand, ApiResponse>
        , IRequestHandler<DeleteLikeFromReplyCommand, ApiResponse>
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
            var (result, replies) = await replyService.AddReplyAsync(request.Content, request.CommentId, request.UserId);
            return result switch
            {
                "CommentNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CommentNotFound]),
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "TheUserYouWantToReplyToNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.TheUserYouWantToReplyToNotFound]),
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

        public async Task<ApiResponse> Handle(UpdateReplyCommand request, CancellationToken cancellationToken)
        {
            var (result, content, since) = await replyService.UpdateReplyAsync(request.Content, request.Id);
            return result switch
            {
                "ReplyNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.ReplyNotFound]),
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "YouAreNotTheOwnerOfThisReplyOrYouAreNotTheAdmin" =>
                Forbidden(stringLocalizer[SharedTranslationKeys.YouAreNotTheOwnerOfThisReply]),
                "AnErrorOccurredWhileUpdatingTheReply" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileUpdatingTheReply]),
                "TheReplyHasBeenSuccessfullyUpdated" => Success(new
                {
                    Content = content,
                    Since = since
                }, message: stringLocalizer[SharedTranslationKeys.TheReplyHasBeenSuccessfullyUpdated]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileUpdatingTheReply])
            };
        }

        public async Task<ApiResponse> Handle(AddLikeToReplyCommand request, CancellationToken cancellationToken)
        {
            var result = await replyService.AddLikeAsync(request.Id);
            return result switch
            {
                "ReplyNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.ReplyNotFound]),
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "YouAreAlreadyAddedLikeToThisReply" => Conflict(stringLocalizer[SharedTranslationKeys.YouAreAlreadyAddedLikeToThisReply]),
                "TheLikeProcessForThisReplyFailed" => InternalServerError(stringLocalizer[SharedTranslationKeys.TheLikeProcessForThisReplyFailed]),
                "AnErrorOccurredWhileAddingALikeToTheReply" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingALikeToTheReply]),
                "TheLikeHasBeenAddedToTheReplySuccessfully" =>
                Success(null, message: stringLocalizer[SharedTranslationKeys.TheLikeHasBeenAddedToTheReplySuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingALikeToTheReply])
            };
        }

        public async Task<ApiResponse> Handle(DeleteLikeFromReplyCommand request, CancellationToken cancellationToken)
        {
            var result = await replyService.DeleteLikeAsync(request.Id);
            return result switch
            {
                "ReplyNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.ReplyNotFound]),
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "YouAreNotLikedThisReply" => BadRequest(stringLocalizer[SharedTranslationKeys.YouAreNotLikedThisReply]),
                "AnErrorOccurredWhileRemovingALikeFromTheReply" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileRemovingALikeFromTheReply]),
                "TheLikeHasBeenDeletedFromTheReplySuccessfully" =>
                Success(null, message: stringLocalizer[SharedTranslationKeys.TheLikeHasBeenDeletedFromTheReplySuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileRemovingALikeFromTheReply])
            };
        }
    }
}