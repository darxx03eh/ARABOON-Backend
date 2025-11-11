using Araboon.Core.Bases;
using Araboon.Core.Features.Swipers.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Data.Response.Swipers.Queries;
using Araboon.Service.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Swipers.Queries.Handlers
{
    public class SwiperQueryHandler : ApiResponseHandler
        , IRequestHandler<GetSwipersForHomePageQuery, ApiResponse>
        , IRequestHandler<GetSwipersForDashboardQuery, ApiResponse>
    {
        private readonly ISwiperService swiperService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IMapper mapper;

        public SwiperQueryHandler(ISwiperService swiperService, IStringLocalizer<SharedTranslation> stringLocalizer
            , IMapper mapper)
        {
            this.swiperService = swiperService;
            this.stringLocalizer = stringLocalizer;
            this.mapper = mapper;
        }
        public async Task<ApiResponse> Handle(GetSwipersForHomePageQuery request, CancellationToken cancellationToken)
        {
            var (result, swiper) = await swiperService.GetSwiperForHomePageAsync();
            return result switch
            {
                "SwipersNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.SwipersNotFound]),
                "SwipersFound" =>
                Success(mapper.Map<IList<GetSwiperForHomePageResponse>>(swiper), message: stringLocalizer[SharedTranslationKeys.SwipersFound]),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.SwipersNotFound])
            };
        }

        public async Task<ApiResponse> Handle(GetSwipersForDashboardQuery request, CancellationToken cancellationToken)
        {
            var (result, swipers, meta) = await swiperService.GetSwiperForDashboardAsync();
            return result switch
            {
                "SwipersNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.SwipersNotFound]),
                "SwipersFound" => 
                Success(mapper.Map<IList<GetSwiperForDashboardResponse>>(swipers), meta, stringLocalizer[SharedTranslationKeys.SwipersFound]),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.SwipersNotFound])
            };
        }
    }
}
