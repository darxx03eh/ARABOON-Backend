using Araboon.Core.Bases;
using Araboon.Core.Features.Categories.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Categories.Commands.Handlers
{
    public class CategoryCommandHandler : ApiResponseHandler
        , IRequestHandler<AddNewCategoryCommand, ApiResponse>
        , IRequestHandler<DeleteCategoryCommand, ApiResponse>
        , IRequestHandler<ActivateCategoryCommand, ApiResponse>
        , IRequestHandler<DeActivateCategoryCommand, ApiResponse>
        , IRequestHandler<UpdateCategoryCommand, ApiResponse>
    {
        private readonly ICategoryService categoryService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public CategoryCommandHandler(ICategoryService categoryService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.categoryService = categoryService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(AddNewCategoryCommand request, CancellationToken cancellationToken)
        {
            var (result, id) = await categoryService.AddNewCategoryAsync(request.CategoryNameEn, request.CategoryNameAr);
            return result switch
            {
                "AnErrorOccurredWhileAddingtheCategory" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingtheCategory]),
                "CategoryAddedSuccessfully" => Created(new
                {
                    Id = id,
                    CategoryNameEn = request.CategoryNameEn,
                    CategoryNameAr = request.CategoryNameAr,
                }, message: stringLocalizer[SharedTranslationKeys.CategoryAddedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingtheCategory])
            };
        }

        public async Task<ApiResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var result = await categoryService.DeleteCategoryAsync(request.Id);
            return result switch
            {
                "CategoryNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CategoryNotFound]),
                "AnErrorOccurredWhileDeletingtheCategory" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingtheCategory]),
                "CategoryDeletedSuccessfully" => Deleted(stringLocalizer[SharedTranslationKeys.CategoryDeletedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingtheCategory])
            };
        }

        public async Task<ApiResponse> Handle(ActivateCategoryCommand request, CancellationToken cancellationToken)
        {
            var result = await categoryService.ActivateCategoryAsync(request.Id);
            return result switch
            {
                "CategoryNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CategoryNotFound]),
                "CategoryAlreadyActive" => Conflict(stringLocalizer[SharedTranslationKeys.CategoryAlreadyActive]),
                "AnErrorOccurredWhileActivateTheCategory" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileActivateTheCategory]),
                "CategoryActivateSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.CategoryActivateSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileActivateTheCategory])
            };
        }

        public async Task<ApiResponse> Handle(DeActivateCategoryCommand request, CancellationToken cancellationToken)
        {
            var result = await categoryService.DeActivateCategoryAsync(request.Id);
            return result switch
            {
                "CategoryNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CategoryNotFound]),
                "CategoryAlreadDeActive" => Conflict(stringLocalizer[SharedTranslationKeys.CategoryAlreadDeActive]),
                "AnErrorOccurredWhileDeActivateTheCategory" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeActivateTheCategory]),
                "CategoryDeActivateSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.CategoryDeActivateSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeActivateTheCategory])
            };
        }

        public async Task<ApiResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var result = await categoryService.UpdateCategoryAsync(request.Id, request.CategoryNameEn, request.CategoryNameAr);
            return result switch
            {
                "CategoryNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CategoryNotFound]),
                "AnErrorOccurredWhileUpdatingTheCategory" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileUpdatingTheCategory]),
                "CategoryUpdatedSuccessfully" => Success(request, message: stringLocalizer[SharedTranslationKeys.CategoryUpdatedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileUpdatingTheCategory])
            };
        }
    }
}
