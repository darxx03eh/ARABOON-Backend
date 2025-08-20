using Araboon.Data.Entities;
using Araboon.Data.Enums;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Data.Wrappers;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;

namespace Araboon.Service.Implementations
{
    public class MangaService : IMangaService
    {
        private readonly IMangaRepository mangaRepository;

        public MangaService(IMangaRepository mangaRepository)
        {
            this.mangaRepository = mangaRepository;
        }
        public async Task<(String, Dictionary<String, IList<GetCategoriesHomePageResponse>>?)> GetCategoriesHomePageAsync()
        {
            var (message, list) = await mangaRepository.GetCategoriesHomePageAsync();
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                _ => ("MangaFound", list)
            };
        }
        public async Task<(String, IList<GetHottestMangasResponse>?)> GetHottestMangasAsync()
        {
            var (message, hottestMangas) = await mangaRepository.GetHottestMangasAsync();
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                _ => ("MangaFound", hottestMangas)
            };
        }
        public async Task<(String, PaginatedResult<GetPaginatedHottestMangaResponse>?)> GetPaginatedHottestMangaAsync(Int32 pageNumber, Int32 pageSize)
        {
            var (message, hottestMangas) = await mangaRepository.GetPaginatedHottestMangaAsync(pageNumber, pageSize);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                _ => ("MangaFound", hottestMangas)
            };
        }
        public async Task<(String, PaginatedResult<GetMangaByCategoryNameResponse>?)> GetMangaByCategoryNameAsync(String category, Int32 pageNumber, Int32 pageSize)
        {
            var (message, mangas) = await mangaRepository.GetMangaByCategoryNameAsync(category, pageNumber, pageSize);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                _ => ("MangaFound", mangas)
            };
        }
        public async Task<(String, PaginatedResult<GetMangaByStatusResponse>?)> GetMangaByStatusAsync(Int32 pageNumber, Int32 pageSize, String status, MangaOrderingEnum orderBy, String? filter)
        {
            var (message, mangas) = await mangaRepository.GetMangaByStatusAsync(pageNumber, pageSize, status, orderBy, filter);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                _ => ("MangaFound", mangas)
            };
        }
        public async Task<(String, Manga?)> GetMangaByIDAsync(Int32 id)
        {
            var manga = await mangaRepository.GetByIdAsync(id);
            if (manga is null)
                return ("MangaNotFound", null);
            return ("MangaFound", manga);
        }
    }
}
