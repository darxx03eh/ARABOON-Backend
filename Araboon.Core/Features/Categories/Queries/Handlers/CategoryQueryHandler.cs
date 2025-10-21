using Araboon.Core.Bases;
using Araboon.Core.Features.Categories.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Data.Response.Categories.Queries;
using Araboon.Service.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Categories.Queries.Handlers
{
    public class CategoryQueryHandler : ApiResponseHandler
        , IRequestHandler<GetCategoriesQuery, ApiResponse>
        , IRequestHandler<GetDashboardCategoriesQuery, ApiResponse>
        , IRequestHandler<GetCategoryByIdQuery, ApiResponse>
    {
        private readonly ICategoryService categoryService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IMapper mapper;

        public CategoryQueryHandler(ICategoryService categoryService, IStringLocalizer<SharedTranslation> stringLocalizer, 
                                    IMapper mapper)
        {
            this.categoryService = categoryService;
            this.stringLocalizer = stringLocalizer;
            this.mapper = mapper;
        }

        public async Task<ApiResponse> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var (result, categories) = await categoryService.GetCategoriesAsync();
            var categoriesMapping = mapper.Map<IList<CategoriesResponse>>(categories);
            return result switch
            {
                "CategoriesNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CategoriesNotFound]),
                "CategoriesFound" => Success(categoriesMapping, message: stringLocalizer[SharedTranslationKeys.CategoriesFound]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileRetrievingTheCategorie])
            };
        }

        public async Task<ApiResponse> Handle(GetDashboardCategoriesQuery request, CancellationToken cancellationToken)
        {
            var (result, categories) = await categoryService.GetDashboardCategoriesAsync(request.PageNumber, request.PageSize, request.search);
            return result switch
            {
                "CategoriesNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CategoriesNotFound]),
                "CategoriesFound" => Success(categories, message: stringLocalizer[SharedTranslationKeys.CategoriesFound]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileRetrievingTheCategorie])
            };
        }

        public async Task<ApiResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var (result, category) = await categoryService.GetCategoryByIdAsync(request.Id);
            return result switch
            {
                "CategoryNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CategoryNotFound]),
                "CategoryFound" => Success(category, message: stringLocalizer[SharedTranslationKeys.CategoryFound]),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.CategoryNotFound])
            };
        }
    }
}
