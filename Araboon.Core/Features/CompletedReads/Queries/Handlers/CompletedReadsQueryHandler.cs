using Araboon.Core.Bases;
using Araboon.Core.Features.CompletedReads.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Service.Implementations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.CompletedReads.Queries.Handlers
{
    public class CompletedReadsQueryHandler : ApiResponseHandler
        , IRequestHandler<GetPaginatedCompletedReadsMangaQuery, ApiResponse>
    {
        private readonly ICompletedReadsService completedReadsService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public CompletedReadsQueryHandler(ICompletedReadsService completedReadsService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.completedReadsService = completedReadsService;
            this.stringLocalizer = stringLocalizer;
        }
        public async Task<ApiResponse> Handle(GetPaginatedCompletedReadsMangaQuery request, CancellationToken cancellationToken)
        {
            var (result, mangas) = await completedReadsService.GetPaginatedCompletedReadsMangaAsync(request.PageNumber, request.PageSize);
            return result switch
            {
                "CompletedReadsServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.CompletedReadsServiceforRegisteredUsersOnly]),
                "ThereAreNoMangaInYourCompletedReadsList" => NoContent(stringLocalizer[SharedTranslationKeys.ThereAreNoMangaInYourCompletedReadsList]),
                "TheMangaWasFoundInYourCompletedReadsList" =>
                Success(mangas, meta: new
                {
                    MangasCount = mangas.TotalCount
                }, message: stringLocalizer[SharedTranslationKeys.TheMangaWasFoundInYourCompletedReadsList]),
                _ => NoContent(stringLocalizer[SharedTranslationKeys.ThereAreNoMangaInYourCompletedReadsList])
            };
        }
    }
}
