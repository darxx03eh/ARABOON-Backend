using Araboon.Data.Entities;
using Araboon.Data.Response.Favorites.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;

namespace Araboon.Service.Implementations
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository favoriteRepository;
        private readonly IMangaRepository mangaRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository, IMangaRepository mangaRepository)
        {
            this.favoriteRepository = favoriteRepository;
            this.mangaRepository = mangaRepository;
        }

        public async Task<String> AddToFavoriteAsync(Int32 mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = favoriteRepository.ExtractUserIdFromToken();
                if (String.IsNullOrEmpty(userId))
                    return "FavoritesServiceforRegisteredUsersOnly";
                var exist = await favoriteRepository.IsMangaExistForUser(mangaId, Int32.Parse(userId));
                if (exist)
                    return "ThisMangaIsAlreadyInYourFavoritesList";
                await favoriteRepository.AddAsync(new Favorite()
                {
                    MangaID = mangaId,
                    UserID = Int32.Parse(userId)
                });
                return "AddedToFavorites";
            }catch(Exception exp)
            {
                return "ThereWasAProblemAddingTofavorites";
            }
        }

        public async Task<(String, PaginatedResult<GetPaginatedFavoritesMangaResponse>?)> GetPaginatedFavoritesMangaAsync(Int32 pageNumber, Int32 pageSize)
        {
            var (message, mangas) = await favoriteRepository.GetPaginatedFavoritesMangaAsync(pageNumber, pageSize);
            return message switch
            {
                "FavoritesServiceforRegisteredUsersOnly" => ("FavoritesServiceforRegisteredUsersOnly", null),
                "ThereAreNoMangaInYourFavoritesList" => ("ThereAreNoMangaInYourFavoritesList", null),
                "TheMangaWasFoundInYourFavoritesList" => ("TheMangaWasFoundInYourFavoritesList", mangas),
                _ => ("ThereAreNoMangaInYourFavoritesList", null)
            };
        }

        public async Task<String> RemoveFromFavoriteAsync(Int32 mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = favoriteRepository.ExtractUserIdFromToken();
                if (String.IsNullOrEmpty(userId))
                    return "FavoritesServiceforRegisteredUsersOnly";
                var exist = await favoriteRepository.IsMangaExistForUser(mangaId, Int32.Parse(userId));
                if (!exist)
                    return "ThisMangaIsNotInYourFavoritesList";
                await favoriteRepository.DeleteAsync(new Favorite()
                {
                    MangaID = mangaId,
                    UserID = Int32.Parse(userId)
                });
                return "RemovedFromFavorites";
            }
            catch (Exception exp)
            {
                return "ThereWasAProblemDeletingFromFavorites";
            }
        }
    }
}
