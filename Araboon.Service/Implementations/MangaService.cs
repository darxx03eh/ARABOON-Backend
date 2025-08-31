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
        public async Task<(string, Dictionary<string, IList<GetCategoriesHomePageResponse>>?)> GetCategoriesHomePageAsync()
        {
            var (message, list) = await mangaRepository.GetCategoriesHomePageAsync();
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", list),
                _ => ("MangaNotFound", null)
            };
        }
        public async Task<(string, IList<GetHottestMangasResponse>?)> GetHottestMangasAsync()
        {
            var (message, hottestMangas) = await mangaRepository.GetHottestMangasAsync();
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", hottestMangas),
                _ => ("MangaNotFound", null)
            };
        }
        public async Task<(string, PaginatedResult<GetPaginatedHottestMangaResponse>?)> GetPaginatedHottestMangaAsync(int pageNumber, int pageSize)
        {
            var (message, hottestMangas) = await mangaRepository.GetPaginatedHottestMangaAsync(pageNumber, pageSize);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", hottestMangas),
                _ => ("MangaNotFound", null)
            };
        }
        public async Task<(string, PaginatedResult<GetMangaByCategoryNameResponse>?)> GetMangaByCategoryNameAsync(string category, int pageNumber, int pageSize)
        {
            var (message, mangas) = await mangaRepository.GetMangaByCategoryNameAsync(category, pageNumber, pageSize);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", mangas),
                _ => ("MangaNotFound", null)
            };
        }
        public async Task<(string, PaginatedResult<GetMangaByStatusResponse>?)> GetMangaByStatusAsync(int pageNumber, int pageSize, string status, MangaOrderingEnum orderBy, string? filter)
        {
            var (message, mangas) = await mangaRepository.GetMangaByStatusAsync(pageNumber, pageSize, status, orderBy, filter);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", mangas),
                _ => ("MangaNotFound", null)
            };
        }
        public async Task<(string, Manga?)> GetMangaByIDAsync(int id)
        {
            var manga = await mangaRepository.GetByIdAsync(id);
            if (manga is null)
                return ("MangaNotFound", null);
            return ("MangaFound", manga);
        }
        public async Task<(string, IList<MangaSearchResponse>?)> SearchAsync(string search)
        {
            var (message, mangas) = await mangaRepository.SearchAsync(search);
            return message switch
            {
                "MangaNotFound" => ("MangaNotFound", null),
                "MangaFound" => ("MangaFound", mangas),
                _ => ("MangaNotFound", null)
            };
        }
    }
}
