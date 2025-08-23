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

        public async Task<string> AddToFavoriteAsync(int mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = favoriteRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return "FavoritesServiceforRegisteredUsersOnly";
                var exist = await favoriteRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (exist)
                    return "ThisMangaIsAlreadyInYourFavoritesList";
                await favoriteRepository.AddAsync(new Favorite()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });
                return "AddedToFavorites";
            }catch(Exception exp)
            {
                return "ThereWasAProblemAddingTofavorites";
            }
        }

        public async Task<(string, PaginatedResult<GetPaginatedFavoritesMangaResponse>?)> GetPaginatedFavoritesMangaAsync(int pageNumber, int pageSize)
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

        public async Task<string> RemoveFromFavoriteAsync(int mangaId)
        {
            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
                return "MangaNotFound";
            try
            {
                var userId = favoriteRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                    return "FavoritesServiceforRegisteredUsersOnly";
                var exist = await favoriteRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (!exist)
                    return "ThisMangaIsNotInYourFavoritesList";
                await favoriteRepository.DeleteAsync(new Favorite()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
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
