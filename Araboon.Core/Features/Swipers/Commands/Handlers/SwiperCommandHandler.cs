using Araboon.Core.Bases;
using Araboon.Core.Features.Swipers.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Data.Response.Swipers.Queries;
using Araboon.Infrastructure.IRepositories;
using Araboon.Service.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Swipers.Commands.Handlers
{
    public class SwiperCommandHandler : ApiResponseHandler
        , IRequestHandler<ActivateSwiperToggleCommand, ApiResponse>
        , IRequestHandler<DeleteExistingSwiperCommand, ApiResponse>
        , IRequestHandler<AddNewSwiperCommand, ApiResponse>
        , IRequestHandler<UploadNewSwiperImageCommand, ApiResponse>
        , IRequestHandler<UpdateSwiperNoteLinkCommand, ApiResponse>
    {
        private readonly ISwiperService swiperService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IMapper mapper;

        public SwiperCommandHandler(ISwiperService swiperService, IStringLocalizer<SharedTranslation> stringLocalizer
            , IMapper mapper)
        {
            this.swiperService = swiperService;
            this.stringLocalizer = stringLocalizer;
            this.mapper = mapper;
        }
        public async Task<ApiResponse> Handle(ActivateSwiperToggleCommand request, CancellationToken cancellationToken)
        {
            var result = await swiperService.ActivateSwiperToggleAsync(request.Id);
            return result switch
            {
                "SwiperToActivateToggleNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.SwiperToActivateToggleNotFound]),
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "CanNotActivateThisSwiperBecauseItIsLinkedToAnInactiveManga" =>
                BadRequest(stringLocalizer[SharedTranslationKeys.CanNotActivateThisSwiperBecauseItIsLinkedToAnInactiveManga]),
                "ActivateSwiperSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.ActivateSwiperSuccessfully]),
                "DeActivateSwiperSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.DeActivateSwiperSuccessfully]),
                "AnErrorOccurredWhileActivatingOrDeActivatingProcess" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileActivatingOrDeActivatingProcess]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileActivatingOrDeActivatingProcess])
            };
        }

        public async Task<ApiResponse> Handle(DeleteExistingSwiperCommand request, CancellationToken cancellationToken)
        {
            var result = await swiperService.DeleteExistingSwiperAsync(request.Id);
            return result switch
            {
                "SwiperNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.SwiperNotFound]),
                "AnErrorOccurredWhileDeleteingSwiperProcess" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeleteingSwiperProcess]),
                "SwiperDeletedSuccessfully" => Success(null, message: stringLocalizer[SharedTranslationKeys.SwiperDeletedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeleteingSwiperProcess])
            };
        }

        public async Task<ApiResponse> Handle(AddNewSwiperCommand request, CancellationToken cancellationToken)
        {
            var (result, swiper) = await swiperService.AddNewSwiperAsync(request.Image, request.Link, request.NoteEn, request.NoteAr);
            return result switch
            {
                "AnErrorOccurredWhileAddingSwiperProcess" => 
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingSwiperProcess]),
                "YouCanNotAddTheSwiperBecauseMangaNotExist" => BadRequest(stringLocalizer[SharedTranslationKeys.YouCanNotAddTheSwiperBecauseMangaNotExist]),
                "SwiperAddedSuccessfully" => 
                Success(mapper.Map<GetSwiperForDashboardResponse>(swiper), message: stringLocalizer[SharedTranslationKeys.SwiperAddedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingSwiperProcess])
            };
        }

        public async Task<ApiResponse> Handle(UploadNewSwiperImageCommand request, CancellationToken cancellationToken)
        {
            var (result, image) = await swiperService.UploadNewSwiperImageAsync(request.Id, request.Image);
            return result switch
            {
                "SwiperNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.SwiperNotFound]),
                "FailedToDeleteOldImageFromCloudinary" => NotFound(stringLocalizer[SharedTranslationKeys.FailedToDeleteOldImageFromCloudinary]),
                "AnErrorOccurredWhileProcessingImageModificationRequest" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileProcessingImageModificationRequest]),
                "TheImageHasBeenChangedSuccessfully" => Success(new
                {
                    Url = image
                }, message: stringLocalizer[SharedTranslationKeys.TheImageHasBeenChangedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileProcessingImageModificationRequest])
            };
        }

        public async Task<ApiResponse> Handle(UpdateSwiperNoteLinkCommand request, CancellationToken cancellationToken)
        {
            var (result, swiper) = await swiperService.UpdateSwiperNoteLinkAsync(request.Id, request.NoteEn, request.NoteAr, request.Link);
            return result switch
            {
                "SwiperNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.SwiperNotFound]),
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "AnErrorOccurredWhileUpdatingSwiperNote" => 
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileUpdatingSwiperNote]),
                "SwiperUpdatedSuccessfully" => Success(
                    mapper.Map<GetSwiperForDashboardResponse>(swiper),
                    message: stringLocalizer[SharedTranslationKeys.SwiperUpdatedSuccessfully]
                ),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileUpdatingSwiperNote])
            };
        }
    }
}
