using Araboon.Core.Bases;
using Araboon.Core.Features.CurrentlyReadings.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Infrastructure.Repositories;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.CurrentlyReadings.Queries.Handlers
{
    public class CurrentlyReadingQueryHandler : ApiResponseHandler
        , IRequestHandler<GetPaginatedCurrentlyReadingMangaQuery, ApiResponse>
    {
        private readonly ICurrentlyReadingService currentlyReadingService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public CurrentlyReadingQueryHandler(ICurrentlyReadingService currentlyReadingService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.currentlyReadingService = currentlyReadingService;
            this.stringLocalizer = stringLocalizer;
        }
        public async Task<ApiResponse> Handle(GetPaginatedCurrentlyReadingMangaQuery request, CancellationToken cancellationToken)
        {
            var (result, mangas) = await currentlyReadingService.GetPaginatedCurrentlyReadingsMangaAsync(request.PageNumber, request.PageSize);
            return result switch
            {
                "CurrentlyReadingServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.CurrentlyReadingServiceforRegisteredUsersOnly]),
                "ThereAreNoMangaInYourCurrentlyReadingList" => NoContent(stringLocalizer[SharedTranslationKeys.ThereAreNoMangaInYourCurrentlyReadingList]),
                "TheMangaWasFoundInYourCurrentlyReadingList" =>
                Success(mangas, meta: new
                {
                    MangasCount = mangas.TotalCount
                }, message: stringLocalizer[SharedTranslationKeys.TheMangaWasFoundInYourCurrentlyReadingList]),
                _ => NoContent(stringLocalizer[SharedTranslationKeys.ThereAreNoMangaInYourCurrentlyReadingList])
            };
        }
    }
}
