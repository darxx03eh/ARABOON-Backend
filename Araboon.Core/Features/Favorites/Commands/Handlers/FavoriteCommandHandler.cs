using Araboon.Core.Bases;
using Araboon.Core.Features.Favorites.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Favorites.Commands.Handlers
{
    public class FavoriteCommandHandler : ApiResponseHandler
        , IRequestHandler<AddToFavoriteCommand, ApiResponse>
        , IRequestHandler<RemoveFromFavoriteCommand, ApiResponse>
    {
        private readonly IFavoriteService favoriteService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public FavoriteCommandHandler(IFavoriteService favoriteService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.favoriteService = favoriteService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(AddToFavoriteCommand request, CancellationToken cancellationToken)
        {
            var result = await favoriteService.AddToFavoriteAsync(request.MangaID);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "FavoritesServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.FavoritesServiceforRegisteredUsersOnly]),
                "ThisMangaIsAlreadyInYourFavoritesList" =>
                Conflict(stringLocalizer[SharedTranslationKeys.ThisMangaIsAlreadyInYourFavoritesList]),
                "AddedToFavorites" => Success(null, message: stringLocalizer[SharedTranslationKeys.AddedToFavorites]),
                "ThereWasAProblemAddingTofavorites"
                => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemAddingTofavorites]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemAddingTofavorites])
            };
        }

        public async Task<ApiResponse> Handle(RemoveFromFavoriteCommand request, CancellationToken cancellationToken)
        {
            var result = await favoriteService.RemoveFromFavoriteAsync(request.MangaID);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "FavoritesServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.FavoritesServiceforRegisteredUsersOnly]),
                "ThisMangaIsNotInYourFavoritesList" =>
                NotFound(stringLocalizer[SharedTranslationKeys.ThisMangaIsNotInYourFavoritesList]),
                "RemovedFromFavorites" => Success(null, message: stringLocalizer[SharedTranslationKeys.RemovedFromFavorites]),
                "ThereWasAProblemDeletingFromFavorites"
                => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemDeletingFromFavorites]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemDeletingFromFavorites])
            };
        }
    }
}
