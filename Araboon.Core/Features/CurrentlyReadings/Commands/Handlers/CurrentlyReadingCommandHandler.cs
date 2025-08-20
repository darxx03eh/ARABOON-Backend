using Araboon.Core.Bases;
using Araboon.Core.Features.CurrentlyReadings.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Service.Implementations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.CurrentlyReadings.Commands.Handlers
{
    public class CurrentlyReadingCommandHandler : ApiResponseHandler
        , IRequestHandler<AddToCurrentlyReadingCommand, ApiResponse>
        , IRequestHandler<RemoveFromCurrentlyReadingCommand, ApiResponse>
    {
        private readonly ICurrentlyReadingService currentlyReadingService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public CurrentlyReadingCommandHandler(ICurrentlyReadingService currentlyReadingService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.currentlyReadingService = currentlyReadingService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(AddToCurrentlyReadingCommand request, CancellationToken cancellationToken)
        {
            var result = await currentlyReadingService.AddToCurrentlyReadingAsync(request.MangaID);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "CurrentlyReadingServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.CurrentlyReadingServiceforRegisteredUsersOnly]),
                "ThisMangaIsAlreadyInYourCurrentlyReadingList" =>
                Conflict(stringLocalizer[SharedTranslationKeys.ThisMangaIsAlreadyInYourCurrentlyReadingList]),
                "AddedToCurrentlyReading" => Success(null, message: stringLocalizer[SharedTranslationKeys.AddedToCurrentlyReading]),
                "ThereWasAProblemAddingToCurrentlyReading"
                => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemAddingToCurrentlyReading]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemAddingToCurrentlyReading])
            };
        }
        public async Task<ApiResponse> Handle(RemoveFromCurrentlyReadingCommand request, CancellationToken cancellationToken)
        {
            var result = await currentlyReadingService.RemoveFromCurrentlyReadingAsync(request.MangaID);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "CurrentlyReadingServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.CurrentlyReadingServiceforRegisteredUsersOnly]),
                "ThisMangaIsNotInYourCurrentlyReadingList" =>
                NotFound(stringLocalizer[SharedTranslationKeys.ThisMangaIsNotInYourCurrentlyReadingList]),
                "RemovedFromCurrentlyReading" => Success(null, message: stringLocalizer[SharedTranslationKeys.RemovedFromCurrentlyReading]),
                "ThereWasAProblemDeletingFromCurrentlyReading"
                => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemDeletingFromCurrentlyReading]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemDeletingFromCurrentlyReading])
            };
        }
    }
}
