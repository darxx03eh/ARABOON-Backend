using Araboon.Core.Bases;
using Araboon.Core.Features.Mangas.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Service.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Mangas.Queries.Handlers
{
    internal class MangaQueryHandler : ApiResponseHandler
        , IRequestHandler<GetCategoriesHomePageQuery, ApiResponse>
        , IRequestHandler<GetHottestMangasQuery, ApiResponse>
        , IRequestHandler<GetMangaByIDQuery, ApiResponse>
        , IRequestHandler<GetMangaByCategoryNameQuery, ApiResponse>
        , IRequestHandler<GetPaginatedHottestMangaQuery, ApiResponse>
        , IRequestHandler<GetMangaByStatusQuery, ApiResponse>
        , IRequestHandler<MangaSearchQuery, ApiResponse>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IMangaService mangaService;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;

        public MangaQueryHandler(IStringLocalizer<SharedTranslation> stringLocalizer, IMangaService mangaService,
                                 IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.mangaService = mangaService;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponse> Handle(GetCategoriesHomePageQuery request, CancellationToken cancellationToken)
        {
            var (result, mangaList, categories) = await mangaService.GetCategoriesHomePageAsync();
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "MangaFound" => Success(mangaList, message: stringLocalizer[SharedTranslationKeys.MangaFound], meta: new
                {
                    Categories = categories
                }),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound])
            };
        }

        public async Task<ApiResponse> Handle(GetHottestMangasQuery request, CancellationToken cancellationToken)
        {
            var (result, hottestMangas) = await mangaService.GetHottestMangasAsync();
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "MangaFound" => Success(hottestMangas, message: stringLocalizer[SharedTranslationKeys.MangaFound]),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound])
            };
        }
        public async Task<ApiResponse> Handle(GetMangaByIDQuery request, CancellationToken cancellationToken)
        {
            var (result, manga, url) = await mangaService.GetMangaByIDAsync(request.ID);
            var httpContext = httpContextAccessor.HttpContext;
            var langHeader = httpContext?.Request.Headers["Accept-Language"].ToString();
            var lang = "en";
            if (!string.IsNullOrEmpty(langHeader))
                lang = langHeader.Split(',')[0].Split('-')[0];
            var mangaResponse = mapper.Map<GetMangaByIDResponse>(manga, opts => opts.Items["lang"] = lang);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "MangaFound" => Success(mangaResponse, message: stringLocalizer[SharedTranslationKeys.MangaFound], meta: new {
                    url = string.Join('-', url.Split(' '))
                }),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound])
            };
        }
        public async Task<ApiResponse> Handle(GetMangaByCategoryNameQuery request, CancellationToken cancellationToken)
        {
            var (result, mangas) = await mangaService.GetMangaByCategoryNameAsync(request.CategoryName, request.PageNumber, request.PageSize);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "MangaFound" => Success(mangas, message: stringLocalizer[SharedTranslationKeys.MangaFound]),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound])
            };
        }
        public async Task<ApiResponse> Handle(GetPaginatedHottestMangaQuery request, CancellationToken cancellationToken)
        {
            var (result, hottestMangas) = await mangaService.GetPaginatedHottestMangaAsync(request.PageNumber, request.PageSize);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "MangaFound" => Success(hottestMangas, message: stringLocalizer[SharedTranslationKeys.MangaFound]),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound])
            };
        }
        public async Task<ApiResponse> Handle(GetMangaByStatusQuery request, CancellationToken cancellationToken)
        {
            var (result, mangas) = await mangaService.GetMangaByStatusAsync(request.PageNumber, request.PageSize, request.Status, request.OrderBy, request.Filter);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "MangaFound" => Success(mangas, message: stringLocalizer[SharedTranslationKeys.MangaFound]),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound])
            };
        }

        public async Task<ApiResponse> Handle(MangaSearchQuery request, CancellationToken cancellationToken)
        {
            var search = string.IsNullOrWhiteSpace(request.Search) ? "" : request.Search.Trim();
            var (result, mangas) = await mangaService.SearchAsync(search);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "MangaFound" => Success(mangas, message: stringLocalizer[SharedTranslationKeys.MangaFound]),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound])
            };
        }
    }
}
