using Araboon.Core.Bases;
using Araboon.Core.Features.ReadingLaters.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Service.Implementations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.ReadingLaters.Commands.Handlers
{
    public class ReadingLaterCommandHandler : ApiResponseHandler
        , IRequestHandler<AddToReadingLaterCommand, ApiResponse>
        , IRequestHandler<RemoveFromReadingLaterCommand, ApiResponse>
    {
        private readonly IReadingLaterService readingLaterService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public ReadingLaterCommandHandler(IReadingLaterService readingLaterService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.readingLaterService = readingLaterService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(AddToReadingLaterCommand request, CancellationToken cancellationToken)
        {
            var result = await readingLaterService.AddToReadingLaterAsync(request.MangaID);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "ReadingLaterServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.ReadingLaterServiceforRegisteredUsersOnly]),
                "ThisMangaIsAlreadyInYourReadingLaterList" =>
                Conflict(stringLocalizer[SharedTranslationKeys.ThisMangaIsAlreadyInYourReadingLaterList]),
                "AddedToReadingLater" => Success(null, message: stringLocalizer[SharedTranslationKeys.AddedToReadingLater]),
                "ThereWasAProblemAddingToReadingLater"
                => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemAddingToReadingLater]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemAddingToReadingLater])
            };
        }
        public async Task<ApiResponse> Handle(RemoveFromReadingLaterCommand request, CancellationToken cancellationToken)
        {
            var result = await readingLaterService.RemoveFromReadingLaterAsync(request.MangaID);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "ReadingLaterServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.ReadingLaterServiceforRegisteredUsersOnly]),
                "ThisMangaIsNotInYourReadingLaterList" =>
                NotFound(stringLocalizer[SharedTranslationKeys.ThisMangaIsNotInYourReadingLaterList]),
                "RemovedFromReadingLater" => Success(null, message: stringLocalizer[SharedTranslationKeys.RemovedFromReadingLater]),
                "ThereWasAProblemDeletingFromReadingLater"
                => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemDeletingFromReadingLater]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemDeletingFromReadingLater])
            };
        }
    }
}
