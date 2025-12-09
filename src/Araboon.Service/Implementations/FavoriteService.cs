using Araboon.Data.Entities;
using Araboon.Data.Response.Favorites.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using Microsoft.Extensions.Logging;

namespace Araboon.Service.Implementations
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository favoriteRepository;
        private readonly IMangaRepository mangaRepository;
        private readonly ILogger<FavoriteService> logger;

        public FavoriteService(IFavoriteRepository favoriteRepository, IMangaRepository mangaRepository, ILogger<FavoriteService> logger)
        {
            this.favoriteRepository = favoriteRepository;
            this.mangaRepository = mangaRepository;
            this.logger = logger;
        }

        public async Task<string> AddToFavoriteAsync(int mangaId)
        {
            logger.LogInformation(
                "Adding manga to favorites - إضافة مانجا إلى المفضلة | MangaId: {MangaId}",
                mangaId);

            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning(
                    "Manga not found - المانجا غير موجودة | MangaId: {MangaId}",
                    mangaId);

                return "MangaNotFound";
            }

            try
            {
                var userId = favoriteRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning(
                        "Unauthorized access to favorites - محاولة إضافة للمفضلة بدون تسجيل دخول");

                    return "FavoritesServiceforRegisteredUsersOnly";
                }

                var exist = await favoriteRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (exist)
                {
                    logger.LogInformation(
                        "Manga already in favorites - المانجا موجودة بالفعل في المفضلة | UserId: {UserId}, MangaId: {MangaId}",
                        userId, mangaId);

                    return "ThisMangaIsAlreadyInYourFavoritesList";
                }

                await favoriteRepository.AddAsync(new Favorite()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });

                logger.LogInformation(
                    "Manga added to favorites successfully - تم إضافة المانجا إلى المفضلة بنجاح | UserId: {UserId}, MangaId: {MangaId}",
                    userId, mangaId);

                return "AddedToFavorites";
            }
            catch (Exception exp)
            {
                logger.LogError(
                    exp,
                    "Error adding manga to favorites - خطأ أثناء إضافة المانجا للمفضلة | MangaId: {MangaId}",
                    mangaId);

                return "ThereWasAProblemAddingTofavorites";
            }
        }

        public async Task<(string, PaginatedResult<GetPaginatedFavoritesMangaResponse>?)> GetPaginatedFavoritesMangaAsync(int pageNumber, int pageSize)
        {
            logger.LogInformation(
                "Fetching paginated favorites - جلب المفضلة مع الصفحات | Page: {Page}, Size: {Size}",
                pageNumber, pageSize);

            var flag = await favoriteRepository.IsAdmin();
            var (message, mangas) = await favoriteRepository.GetPaginatedFavoritesMangaAsync(pageNumber, pageSize, flag);

            logger.LogInformation(
                "Favorites fetch result - نتيجة جلب المفضلة | Message: {Message}",
                message);

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
            logger.LogInformation(
                "Removing manga from favorites - إزالة مانجا من المفضلة | MangaId: {MangaId}",
                mangaId);

            var manga = await mangaRepository.GetByIdAsync(mangaId);
            if (manga is null)
            {
                logger.LogWarning(
                    "Manga not found - المانجا غير موجودة | MangaId: {MangaId}",
                    mangaId);

                return "MangaNotFound";
            }

            try
            {
                var userId = favoriteRepository.ExtractUserIdFromToken();
                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning(
                        "Unauthorized removal attempt - محاولة حذف من المفضلة بدون تسجيل دخول");

                    return "FavoritesServiceforRegisteredUsersOnly";
                }

                var exist = await favoriteRepository.IsMangaExistForUser(mangaId, int.Parse(userId));
                if (!exist)
                {
                    logger.LogWarning(
                        "Manga not in favorites - المانجا غير موجودة في المفضلة | UserId: {UserId}, MangaId: {MangaId}",
                        userId, mangaId);

                    return "ThisMangaIsNotInYourFavoritesList";
                }

                await favoriteRepository.DeleteAsync(new Favorite()
                {
                    MangaID = mangaId,
                    UserID = int.Parse(userId)
                });

                logger.LogInformation(
                    "Manga removed from favorites successfully - تم إزالة المانجا من المفضلة | UserId: {UserId}, MangaId: {MangaId}",
                    userId, mangaId);

                return "RemovedFromFavorites";
            }
            catch (Exception exp)
            {
                logger.LogError(
                    exp,
                    "Error removing manga from favorites - خطأ أثناء إزالة المانجا من المفضلة | MangaId: {MangaId}",
                    mangaId);

                return "ThereWasAProblemDeletingFromFavorites";
            }
        }
    }
}