using Araboon.Core.Bases;
using Araboon.Core.Features.ReadingLaters.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Service.Implementations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.ReadingLaters.Queries.Handlers
{
    public class ReadingLaterQueryHandler : ApiResponseHandler
        , IRequestHandler<GetPaginatedReadingLaterMangaQuery, ApiResponse>
    {
        private readonly IReadingLaterService readingLaterService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public ReadingLaterQueryHandler(IReadingLaterService readingLaterService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.readingLaterService = readingLaterService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(GetPaginatedReadingLaterMangaQuery request, CancellationToken cancellationToken)
        {
            var (result, mangas) = await readingLaterService.GetPaginatedReadingLaterMangaAsync(request.PageNumber, request.PageSize);
            return result switch
            {
                "ReadingLaterServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.ReadingLaterServiceforRegisteredUsersOnly]),
                "ThereAreNoMangaInYourReadingLaterList" => NoContent(stringLocalizer[SharedTranslationKeys.ThereAreNoMangaInYourReadingLaterList]),
                "TheMangaWasFoundInYourReadingLaterList" =>
                Success(mangas, meta: new
                {
                    MangasCount = mangas.TotalCount
                }, message: stringLocalizer[SharedTranslationKeys.TheMangaWasFoundInYourReadingLaterList]),
                _ => NoContent(stringLocalizer[SharedTranslationKeys.ThereAreNoMangaInYourReadingLaterList])
            };
        }
    }
}
