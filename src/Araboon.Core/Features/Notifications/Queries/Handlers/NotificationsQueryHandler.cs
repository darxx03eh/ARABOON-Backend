using Araboon.Core.Bases;
using Araboon.Core.Features.Notifications.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Service.Implementations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Notifications.Queries.Handlers
{
    public class NotificationsQueryHandler : ApiResponseHandler
        , IRequestHandler<GetPaginatedNotificationsMangaQuery, ApiResponse>
    {
        private readonly INotificationsService notificationsService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public NotificationsQueryHandler(INotificationsService notificationsService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.notificationsService = notificationsService;
            this.stringLocalizer = stringLocalizer;
        }
        public async Task<ApiResponse> Handle(GetPaginatedNotificationsMangaQuery request, CancellationToken cancellationToken)
        {
            var (result, mangas) = await notificationsService.GetPaginatedNotificationsMangaAsync(request.PageNumber, request.PageSize);
            return result switch
            {
                "NotificationsServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.NotificationsServiceforRegisteredUsersOnly]),
                "ThereAreNoMangaInYourNotificationsList" => NoContent(stringLocalizer[SharedTranslationKeys.ThereAreNoMangaInYourNotificationsList]),
                "TheMangaWasFoundInYourNotificationsList" =>
                Success(mangas, meta: new
                {
                    MangasCount = mangas.TotalCount
                }, message: stringLocalizer[SharedTranslationKeys.TheMangaWasFoundInYourNotificationsList]),
                _ => NoContent(stringLocalizer[SharedTranslationKeys.ThereAreNoMangaInYourNotificationsList])
            };
        }
    }
}
