using Araboon.Core.Bases;
using Araboon.Core.Features.Ratings.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Ratings.Commands.Handlers
{
    public class RatingCommandHandler : ApiResponseHandler
        , IRequestHandler<AddUpdateRatingsCommand, ApiResponse>
        , IRequestHandler<DeleteRatingsCommand, ApiResponse>
    {
        private readonly IRatingService ratingService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public RatingCommandHandler(IRatingService ratingService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.ratingService = ratingService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(AddUpdateRatingsCommand request, CancellationToken cancellationToken)
        {
            var (result, rate, id, newRate) = await ratingService.RateAsync(request.MangaId, request.Rate);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "AnErrorOccurredWhileAddingTheRate" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingTheRate]),
                "TheRateHasBeenAddedSuccessfully" => Success(new
                {
                    Id = id,
                    Rate = rate,
                    NewRate = newRate,
                }, message: stringLocalizer[SharedTranslationKeys.TheRateHasBeenAddedSuccessfully]),
                "TheRateHasBeenModifiedSuccessfully" => Success(new
                {
                    Id = id,
                    Rate = rate,
                    NewRate = newRate,
                }, message: stringLocalizer[SharedTranslationKeys.TheRateHasBeenModifiedSuccessfully]),
                "AnErrorOccurredWhileModifyingTheRate" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileModifyingTheRate]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredDuringTheRateProcess])
            };
        }

        public async Task<ApiResponse> Handle(DeleteRatingsCommand request, CancellationToken cancellationToken)
        {
            var (result, rate) = await ratingService.DeleteRateAsync(request.Id);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "RateNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.RateNotFound]),
                "ThisRateDoNotBelongToYou" => Forbidden(stringLocalizer[SharedTranslationKeys.ThisRateDoNotBelongToYou]),
                "TheRateHasBeenSuccessfullyDeleted" => Success(new
                {
                    NewRate = rate,
                }, message: stringLocalizer[SharedTranslationKeys.TheRateHasBeenSuccessfullyDeleted]),
                "AnErrorOccurredWhileDeletingTheRate" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheRate]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheRate])
            };
        }
    }
}
