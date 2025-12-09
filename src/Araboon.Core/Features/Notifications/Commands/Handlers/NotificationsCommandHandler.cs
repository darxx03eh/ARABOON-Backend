using Araboon.Core.Bases;
using Araboon.Core.Features.Notifications.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Service.Implementations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Notifications.Commands.Handlers
{
    public class NotificationsCommandHandler : ApiResponseHandler
        , IRequestHandler<AddToNotificationsCommand, ApiResponse>
        , IRequestHandler<RemoveFromNotificationsCommand, ApiResponse>
    {
        private readonly INotificationsService notificationsService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public NotificationsCommandHandler(INotificationsService notificationsService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.notificationsService = notificationsService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(AddToNotificationsCommand request, CancellationToken cancellationToken)
        {
            var result = await notificationsService.AddToNotificationsAsync(request.MangaID);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "NotificationsServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.NotificationsServiceforRegisteredUsersOnly]),
                "ThisMangaIsAlreadyInYourNotificationsList" =>
                Conflict(stringLocalizer[SharedTranslationKeys.ThisMangaIsAlreadyInYourNotificationsList]),
                "AddedToNotifications" => Success(null, message: stringLocalizer[SharedTranslationKeys.AddedToNotifications]),
                "ThereWasAProblemAddingToNotifications"
                => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemAddingToNotifications]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemAddingToNotifications])
            };
        }

        public async Task<ApiResponse> Handle(RemoveFromNotificationsCommand request, CancellationToken cancellationToken)
        {
            var result = await notificationsService.RemoveFromNotificationsAsync(request.MangaID);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "NotificationsServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.NotificationsServiceforRegisteredUsersOnly]),
                "ThisMangaIsNotInYourNotificationsList" =>
                NotFound(stringLocalizer[SharedTranslationKeys.ThisMangaIsNotInYourNotificationsList]),
                "RemovedFromNotifications" => Success(null, message: stringLocalizer[SharedTranslationKeys.RemovedFromNotifications]),
                "ThereWasAProblemDeletingFromNotifications"
                => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemDeletingFromNotifications]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemDeletingFromNotifications])
            };
        }
    }
}
