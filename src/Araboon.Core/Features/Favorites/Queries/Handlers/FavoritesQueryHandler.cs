using Araboon.Core.Bases;
using Araboon.Core.Features.Favorites.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Favorites.Queries.Handlers
{
    public class FavoritesQueryHandler : ApiResponseHandler
        , IRequestHandler<GetPaginatedFavoritesMangaQuery, ApiResponse>
    {
        private readonly IFavoriteService favoriteService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public FavoritesQueryHandler(IFavoriteService favoriteService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.favoriteService = favoriteService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(GetPaginatedFavoritesMangaQuery request, CancellationToken cancellationToken)
        {
            var (result, mangas) = await favoriteService.GetPaginatedFavoritesMangaAsync(request.PageNumber, request.PageSize);
            return result switch
            {
                "FavoritesServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.FavoritesServiceforRegisteredUsersOnly]),
                "ThereAreNoMangaInYourFavoritesList" => NoContent(stringLocalizer[SharedTranslationKeys.ThereAreNoMangaInYourFavoritesList]),
                "TheMangaWasFoundInYourFavoritesList" =>
                Success(mangas, meta: new
                {
                    MangasCount = mangas.TotalCount
                }, message: stringLocalizer[SharedTranslationKeys.TheMangaWasFoundInYourFavoritesList]),
                _ => NoContent(stringLocalizer[SharedTranslationKeys.ThereAreNoMangaInYourFavoritesList])
            };
        }
    }
}
