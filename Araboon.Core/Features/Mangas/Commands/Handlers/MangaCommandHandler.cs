using Araboon.Core.Bases;
using Araboon.Core.Features.Mangas.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Mangas.Commands.Handlers
{
    public class MangaCommandHandler : ApiResponseHandler
        , IRequestHandler<AddNewMangaCommand, ApiResponse>
        , IRequestHandler<DeleteMangaCommand, ApiResponse>
        , IRequestHandler<DeleteMangaImageCommand, ApiResponse>
        , IRequestHandler<UploadNewMangaImageCommand, ApiResponse>
        , IRequestHandler<MakeArabicAvailableOrUnAvailableCommand, ApiResponse>
        , IRequestHandler<MakeEnglishAvailableOrUnAvailableCommand, ApiResponse>
        , IRequestHandler<ActivateOrDeActivateMangaCommand, ApiResponse>
        , IRequestHandler<UpdateMangaCommand, ApiResponse>
    {
        private readonly IMangaService mangaService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public MangaCommandHandler(IMangaService mangaService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.mangaService = mangaService;
            this.stringLocalizer = stringLocalizer;
        }
        public async Task<ApiResponse> Handle(AddNewMangaCommand request, CancellationToken cancellationToken)
        {
            var (result, manga) = await mangaService.AddNewMangaAsync(request);
            return result switch
            {
                "CategoryNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CategoryNotFound]),
                "ThereWasAProblemAddingTheManga" => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemAddingTheManga]),
                "AnErrorOccurredWhileAddingTheImageForManga" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingTheImageForManga]),
                "MangaAddedSuccessfully" => Created(manga, message: stringLocalizer[SharedTranslationKeys.MangaAddedSuccessfully]),
                "AnErrorOccurredWhileAddingTheManga" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingTheManga]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingTheManga])
            };
        }

        public async Task<ApiResponse> Handle(DeleteMangaCommand request, CancellationToken cancellationToken)
        {
            var result = await mangaService.DeleteMangaAsync(request.Id);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.NotFound]),
                "AnErrorOccurredWhileDeletingTheManga" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheManga]),
                "MangaDeletedSuccessfully" => Deleted(stringLocalizer[SharedTranslationKeys.MangaDeletedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheManga])
            };
        }

        public async Task<ApiResponse> Handle(DeleteMangaImageCommand request, CancellationToken cancellationToken)
        {
            var result = await mangaService.DeleteMangaImageAsync(request.Id);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "ThereIsNoImageToDelete" => NotFound(stringLocalizer[SharedTranslationKeys.ThereIsNoImageToDelete]),
                "FailedToDeleteImageFromCloudinary" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.FailedToDeleteImageFromCloudinary]),
                "ImageHasBeenSuccessfullyDeleted" => Deleted(stringLocalizer[SharedTranslationKeys.ImageHasBeenSuccessfullyDeleted]),
                "AnErrorOccurredWhileDeletingTheImage" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheImage]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheImage])
            };
        }

        public async Task<ApiResponse> Handle(UploadNewMangaImageCommand request, CancellationToken cancellationToken)
        {
            var (result, imageUrl) = await mangaService.UploadMangaImageAsync(request.Id, request.Image);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "FailedToDeleteOldImageFromCloudinary" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.FailedToDeleteOldImageFromCloudinary]),
                "TheImageHasBeenChangedSuccessfully" => Success(new
                {
                    ImageUrl = imageUrl,
                }, message: stringLocalizer[SharedTranslationKeys.TheImageHasBeenChangedSuccessfully]),
                "AnErrorOccurredWhileProcessingImageModificationRequest" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileProcessingImageModificationRequest]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileProcessingImageModificationRequest])
            };
        }

        public async Task<ApiResponse> Handle(MakeArabicAvailableOrUnAvailableCommand request, CancellationToken cancellationToken)
        {
            var result = await mangaService.MakeArabicAvailableOrUnAvailableAsync(request.Id);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "MakeArabicAvilableForThisMangaSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.MakeArabicAvilableForThisMangaSuccessfully]),
                "MakeArabicNotAvilableForThisMangaSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.MakeArabicNotAvilableForThisMangaSuccessfully]),
                "AnErrorOccurredWhileMakingArabicAvilableOrNotAvilableProcess" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileMakingArabicAvilableOrNotAvilableProcess]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileMakingArabicAvilableOrNotAvilableProcess])
            };
        }
        public async Task<ApiResponse> Handle(MakeEnglishAvailableOrUnAvailableCommand request, CancellationToken cancellationToken)
        {
            var result = await mangaService.MakeEnglishAvailableOrUnAvailableAsync(request.Id);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "MakeEnglishAvilableForThisMangaSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.MakeEnglishAvilableForThisMangaSuccessfully]),
                "MakeEnglishNotAvilableForThisMangaSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.MakeEnglishNotAvilableForThisMangaSuccessfully]),
                "AnErrorOccurredWhileMakingEnglishAvilableOrNotAvilableProcess" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileMakingEnglishAvilableOrNotAvilableProcess]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileMakingEnglishAvilableOrNotAvilableProcess])
            };
        }
        public async Task<ApiResponse> Handle(ActivateOrDeActivateMangaCommand request, CancellationToken cancellationToken)
        {
            var result = await mangaService.ActivateAndDeActivateMangaAsync(request.Id);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "ActivateMangaSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.ActivateMangaSuccessfully]),
                "DeActivateMangaSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.DeActivateMangaSuccessfully]),
                "AnErrorOccurredWhileActivatingOrDeActivatingProcess" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileActivatingOrDeActivatingProcess]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileActivatingOrDeActivatingProcess])
            };
        }
        public async Task<ApiResponse> Handle(UpdateMangaCommand request, CancellationToken cancellationToken)
        {
            var (result, manga) = await mangaService.UpdateExistMangaAsync(request, request.MangaId);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "CategoryNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CategoryNotFound]),
                "MangaUpdatingSuccessfully" => Success(manga, message: stringLocalizer[SharedTranslationKeys.MangaUpdatingSuccessfully]),
                "AnErrorOccurredWhileUpdatingTheManga" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileUpdatingTheManga]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileUpdatingTheManga])
            };
        }
    }
}
