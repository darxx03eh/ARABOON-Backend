using Araboon.Core.Bases;
using Araboon.Core.Features.Ratings.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Ratings.Queries.Handlers
{
    public class RatingQueryHandler : ApiResponseHandler
        , IRequestHandler<GetRatingForMangaQuery, ApiResponse>
    {
        private readonly IRatingService ratingService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public RatingQueryHandler(IRatingService ratingService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.ratingService = ratingService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(GetRatingForMangaQuery request, CancellationToken cancellationToken)
        {
            var (result, rate) = await ratingService.GetRatingsForMangaAsync(request.Id);
            return result switch
            {
                "UserNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.UserNotFound]),
                "RateNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.RateNotFound]),
                "RateFound" => Success(rate, message: stringLocalizer[SharedTranslationKeys.RateFound]),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.RateNotFound])
            };
        }
    }
}
