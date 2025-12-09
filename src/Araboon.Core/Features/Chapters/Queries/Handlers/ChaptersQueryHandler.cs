using Araboon.Core.Bases;
using Araboon.Core.Features.Chapters.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Data.Entities;
using Araboon.Data.Response.Chapters.Queries;
using Araboon.Data.Response.Mangas.Queries;
using Araboon.Service.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Chapters.Queries.Handlers
{
    public class ChaptersQueryHandler : ApiResponseHandler
        , IRequestHandler<GetChaptersForSpecificMangaByLanguageQuery, ApiResponse>
    {
        private readonly IChapterService chapterService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public ChaptersQueryHandler(IChapterService chapterService, IStringLocalizer<SharedTranslation> stringLocalizer
                                  , IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.chapterService = chapterService;
            this.stringLocalizer = stringLocalizer;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }

        public async Task<ApiResponse> Handle(GetChaptersForSpecificMangaByLanguageQuery request, CancellationToken cancellationToken)
        {
            var (result, chapters, isArabicAvailable, isEnglishAvailable) = await chapterService.
                GetChaptersForSpecificMangaByLanguage(request.MangaID, request.Language);
            var httpContext = httpContextAccessor.HttpContext;
            var langHeader = httpContext?.Request.Headers["Accept-Language"].ToString();
            var lang = "en";
            if (!string.IsNullOrEmpty(langHeader))
                lang = langHeader.Split(',')[0].Split('-')[0];
            var chaptersResponse = mapper.Map<IList<ChaptersResponse>>(chapters, opts => opts.Items["lang"] = lang);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "TheLanguageYouRequestedIsNotAvailableForThisManga" =>
                NotFound(stringLocalizer[SharedTranslationKeys.TheLanguageYouRequestedIsNotAvailableForThisManga]),
                "ThereAreNoChaptersYet" => NotFound(stringLocalizer[SharedTranslationKeys.ThereAreNoChaptersYet]),
                "TheChaptersWereFound" => Success(chaptersResponse,new
                {
                    IsArabicAvailable = isArabicAvailable,
                    IsEnglishAvailable = isEnglishAvailable,
                }, message: stringLocalizer[SharedTranslationKeys.TheChaptersWereFound]),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.ThereAreNoChaptersYet])
            };
        }
    }
}
