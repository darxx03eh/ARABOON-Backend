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
        , IRequestHandler<MakeArabicAvailableCommand, ApiResponse>
        , IRequestHandler<MakeArabicUnAvailableCommand, ApiResponse>
        , IRequestHandler<MakeEnglishAvailableCommand, ApiResponse>
        , IRequestHandler<MakeEnglishUnAvailableCommand, ApiResponse>
        , IRequestHandler<ActivateMangaCommand, ApiResponse>
        , IRequestHandler<DeActivateMangaCommand, ApiResponse>
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
            var (result, id, imageUrl) = await mangaService.AddNewMangaAsync(request);
            return result switch
            {
                "CategoryNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CategoryNotFound]),
                "ThereWasAProblemAddingTheManga" => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemAddingTheManga]),
                "AnErrorOccurredWhileAddingTheImageForManga" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingTheImageForManga]),
                "MangaAddedSuccessfully" => Created(new
                {
                    Id = id,
                    ImageUrl = imageUrl,
                }, message: stringLocalizer[SharedTranslationKeys.MangaAddedSuccessfully]),
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

        public async Task<ApiResponse> Handle(MakeArabicAvailableCommand request, CancellationToken cancellationToken)
        {
            var result = await mangaService.MakeArabicAvailableAsync(request.Id);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "ArabicAvailableForThisMangaAlread" => Conflict(stringLocalizer[SharedTranslationKeys.ArabicAvailableForThisMangaAlready]),
                "MakeArabicAvilableForThisMangaSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.MakeArabicAvilableForThisMangaSuccessfully]),
                "AnErrorOccurredWhileMakingArabicAvilableForThisManga" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileMakingArabicAvilableForThisManga]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileMakingArabicAvilableForThisManga])
            };
        }

        public async Task<ApiResponse> Handle(MakeArabicUnAvailableCommand request, CancellationToken cancellationToken)
        {
            var result = await mangaService.MakeArabicUnAvailableAsync(request.Id);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "ArabicNotAvailableForThisMangaAlread" => Conflict(stringLocalizer[SharedTranslationKeys.ArabicNotAvailableForThisMangaAlready]),
                "MakeArabicNotAvilableForThisMangaSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.MakeArabicNotAvilableForThisMangaSuccessfully]),
                "AnErrorOccurredWhileMakingArabicNotAvilableForThisManga" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileMakingArabicNotAvilableForThisManga]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileMakingArabicNotAvilableForThisManga])
            };
        }

        public async Task<ApiResponse> Handle(MakeEnglishAvailableCommand request, CancellationToken cancellationToken)
        {
            var result = await mangaService.MakeEnglishAvailableAsync(request.Id);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "EnglishAvailableForThisMangaAlready" => Conflict(stringLocalizer[SharedTranslationKeys.EnglishAvailableForThisMangaAlready]),
                "MakeEnglishAvilableForThisMangaSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.MakeEnglishAvilableForThisMangaSuccessfully]),
                "AnErrorOccurredWhileMakingEnglishAvilableForThisManga" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileMakingEnglishAvilableForThisManga]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileMakingEnglishAvilableForThisManga])
            };
        }

        public async Task<ApiResponse> Handle(MakeEnglishUnAvailableCommand request, CancellationToken cancellationToken)
        {
            var result = await mangaService.MakeEnglishUnAvailableAsync(request.Id);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "EnglishNotAvailableForThisMangaAlready" => Conflict(stringLocalizer[SharedTranslationKeys.EnglishNotAvailableForThisMangaAlready]),
                "MakeEnglishNotAvilableForThisMangaSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.MakeEnglishNotAvilableForThisMangaSuccessfully]),
                "AnErrorOccurredWhileMakingEnglishNotAvilableForThisManga" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileMakingEnglishNotAvilableForThisManga]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileMakingEnglishNotAvilableForThisManga])
            };
        }

        public async Task<ApiResponse> Handle(ActivateMangaCommand request, CancellationToken cancellationToken)
        {
            var result = await mangaService.ActivateMangaAsync(request.Id);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "MangaAlreadyActive" => Conflict(stringLocalizer[SharedTranslationKeys.MangaAlreadyActive]),
                "ActivateMangaSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.ActivateMangaSuccessfully]),
                "AnErrorOccurredWhileActivateThisManga" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileActivateThisManga]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileActivateThisManga])
            };
        }

        public async Task<ApiResponse> Handle(DeActivateMangaCommand request, CancellationToken cancellationToken)
        {
            var result = await mangaService.DeActivateMangaAsync(request.Id);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "MangaAlreadyDeActive" => Conflict(stringLocalizer[SharedTranslationKeys.MangaAlreadyDeActive]),
                "DeActivateMangaSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.DeActivateMangaSuccessfully]),
                "AnErrorOccurredWhileDeActivateThisManga" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeActivateThisManga]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeActivateThisManga])
            };
        }

        public async Task<ApiResponse> Handle(UpdateMangaCommand request, CancellationToken cancellationToken)
        {
            var result = await mangaService.UpdateExistMangaAsync(request, request.MangaId);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "CategoryNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.CategoryNotFound]),
                "MangaUpdatingSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.MangaUpdatingSuccessfully]),
                "AnErrorOccurredWhileUpdatingTheManga" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileUpdatingTheManga]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileUpdatingTheManga])
            };
        }
    }
}
