using Araboon.Core.Bases;
using Araboon.Core.Features.CompletedReads.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.CompletedReads.Commands.Handlers
{
    public class CompletedReadsCommandHandler : ApiResponseHandler
        , IRequestHandler<AddToCompletedReadsCommand, ApiResponse>
        , IRequestHandler<RemoveFromCompletedReadsCommand, ApiResponse>
    {
        private readonly ICompletedReadsService completedReadsService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public CompletedReadsCommandHandler(ICompletedReadsService completedReadsService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.completedReadsService = completedReadsService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(AddToCompletedReadsCommand request, CancellationToken cancellationToken)
        {
            var result = await completedReadsService.AddToCompletedReadsAsync(request.MangaID);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "CompletedReadsServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.CompletedReadsServiceforRegisteredUsersOnly]),
                "ThisMangaIsAlreadyInYourCompletedReadsList" =>
                Conflict(stringLocalizer[SharedTranslationKeys.ThisMangaIsAlreadyInYourCompletedReadsList]),
                "AddedToCompletedReads" => Success(null, message: stringLocalizer[SharedTranslationKeys.AddedToCompletedReads]),
                "ThereWasAProblemAddingToCompletedReads"
                => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemAddingToCompletedReads]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemAddingToCompletedReads])
            };
        }

        public async Task<ApiResponse> Handle(RemoveFromCompletedReadsCommand request, CancellationToken cancellationToken)
        {
            var result = await completedReadsService.RemoveFromCompletedReadsAsync(request.MangaID);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "CompletedReadsServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.CompletedReadsServiceforRegisteredUsersOnly]),
                "ThisMangaIsNotInYourCompletedReadsList" =>
                NotFound(stringLocalizer[SharedTranslationKeys.ThisMangaIsNotInYourCompletedReadsList]),
                "RemovedFromCompletedReads" => Success(null, message: stringLocalizer[SharedTranslationKeys.RemovedFromCompletedReads]),
                "ThereWasAProblemDeletingFromCompletedReads"
                => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemDeletingFromCompletedReads]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemDeletingFromCompletedReads])
            };
        }
    }
}