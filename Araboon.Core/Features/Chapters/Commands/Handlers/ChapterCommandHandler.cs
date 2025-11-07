using Araboon.Core.Bases;
using Araboon.Core.Features.Chapters.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Data.Entities;
using Araboon.Data.Response.Chapters.Queries;
using Araboon.Service.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Chapters.Commands.Handlers
{
    public class ChapterCommandHandler : ApiResponseHandler
        , IRequestHandler<ChapterReadCommand, ApiResponse>
        , IRequestHandler<AddNewChapterCommand, ApiResponse>
        , IRequestHandler<DeleteExistingChapterCommand, ApiResponse>
        , IRequestHandler<UpdateExistingChapterCommand, ApiResponse>
        , IRequestHandler<UploadChapterImageCommand, ApiResponse>
        , IRequestHandler<UploadChapterImagesCommand, ApiResponse>
    {
        private readonly IChapterService chapterService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public ChapterCommandHandler(IChapterService chapterService, IStringLocalizer<SharedTranslation> stringLocalizer, 
            IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.chapterService = chapterService;
            this.stringLocalizer = stringLocalizer;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }

        public async Task<ApiResponse> Handle(ChapterReadCommand request, CancellationToken cancellationToken)
        {
            var (result, readersCount) = await chapterService.ChapterReadAsync(request.ChapterId);
            return result switch
            {
                "ChapterNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.ChapterNotFound]),
                "ViewsIncreasedBy1" => Success(new
                {
                    ReaderCounts = readersCount,
                }, message: stringLocalizer[SharedTranslationKeys.ViewsIncreasedBy1]),
                "AnErrorOccurredWhileIncreasingTheViewByOne" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileIncreasingTheViewByOne]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileIncreasingTheViewByOne])
            };
        }

        public async Task<ApiResponse> Handle(AddNewChapterCommand request, CancellationToken cancellationToken)
        {
            var (result, chapter, isArabicAvailable, isEnglishAvailable) = await chapterService.AddNewChapterAsync(request);
            var httpContext = httpContextAccessor.HttpContext;
            var langHeader = httpContext?.Request.Headers["Accept-Language"].ToString();
            var lang = "en";
            if (!string.IsNullOrEmpty(langHeader))
                lang = langHeader.Split(',')[0].Split('-')[0];
            var chaptersResponse = mapper.Map<ChaptersResponse>(chapter, opts => opts.Items["lang"] = lang);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "AnErrorOccurredWhileAddingTheImageForChapter" =>
                InternalServerError(SharedTranslationKeys.AnErrorOccurredWhileAddingTheImageForChapter),
                "AnErrorOccurredWhileAddingTheChapter" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingTheChapter]),
                "ChapterAddedSuccessfully" => Success(chaptersResponse ,new
                {
                    IsArabicAvailable = isArabicAvailable,
                    IsEnglishAvailable = isEnglishAvailable,
                }, stringLocalizer[SharedTranslationKeys.ChapterAddedSuccessfully]),
                "ChapterAddedSuccessfullyAndArabicBecameInactiveDueToIncompleteChapters" => 
                Success(chaptersResponse, new
                {
                    IsArabicAvailable = isArabicAvailable,
                    IsEnglishAvailable = isEnglishAvailable,
                }, stringLocalizer[SharedTranslationKeys.ChapterAddedSuccessfullyAndArabicBecameInactiveDueToIncompleteChapters]),
                "ChapterAddedSuccessfullyAndEnglishBecameInactiveDueToIncompleteChapters" => 
                Success(chaptersResponse, new
                {
                    IsArabicAvailable = isArabicAvailable,
                    IsEnglishAvailable = isEnglishAvailable,
                }, stringLocalizer[SharedTranslationKeys.ChapterAddedSuccessfullyAndEnglishBecameInactiveDueToIncompleteChapters]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingTheChapter])
            };
        }

        public async Task<ApiResponse> Handle(DeleteExistingChapterCommand request, CancellationToken cancellationToken)
        {
            var (result, isArabicAvailable, isEnglishAvailable) = await chapterService.DeleteExistingChapterAsync(request.Id);
            return result switch
            {
                "ChapterNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.ChapterNotFound]),
                "ChapterDeletedSuccessfully" => Success(null, new
                {
                    IsArabicAvailable = isArabicAvailable,
                    IsEnglishAvailable = isEnglishAvailable,
                }, stringLocalizer[SharedTranslationKeys.ChapterDeletedSuccessfully]),
                "ChapterDeletedSuccessfullyAndArabicBecameInactiveDueToIncompleteChapters" => 
                Success(null, new
                {
                    IsArabicAvailable = isArabicAvailable,
                    IsEnglishAvailable = isEnglishAvailable,
                }, stringLocalizer[SharedTranslationKeys.ChapterDeletedSuccessfullyAndArabicBecameInactiveDueToIncompleteChapters]),
                "ChapterDeletedSuccessfullyAndEnglishBecameInactiveDueToIncompleteChapters" => 
                Success(null, new
                {
                    IsArabicAvailable = isArabicAvailable,
                    IsEnglishAvailable = isEnglishAvailable,
                }, stringLocalizer[SharedTranslationKeys.ChapterDeletedSuccessfullyAndEnglishBecameInactiveDueToIncompleteChapters]),
                "AnErrorOccurredWhileDeletingTheChapter" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheChapter]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileDeletingTheChapter])
            };
        }

        public async Task<ApiResponse> Handle(UpdateExistingChapterCommand request, CancellationToken cancellationToken)
        {
            var (result, chapter, isArabicAvailable, isEnglishAvailable) = await chapterService.UpdateExistingChapterAsync(
                request.Id, request.ChapterNo, request.ArabicChapterTitle, request.EnglishChapterTitle, request.Language
            );
            var httpContext = httpContextAccessor.HttpContext;
            var langHeader = httpContext?.Request.Headers["Accept-Language"].ToString();
            var lang = "en";
            if (!string.IsNullOrEmpty(langHeader))
                lang = langHeader.Split(',')[0].Split('-')[0];
            var chaptersResponse = mapper.Map<ChaptersResponse>(chapter, opts => opts.Items["lang"] = lang);
            return result switch
            {
                "ChapterNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.ChapterNotFound]),
                "AnErrorOccurredWhileUpdatingTheChapter" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileUpdatingTheChapter]),
                "ChapterUpdatedSuccessfully" => Success(chaptersResponse, new
                {
                    IsArabicAvailable = isArabicAvailable,
                    IsEnglishAvailable = isEnglishAvailable,
                } ,message: stringLocalizer[SharedTranslationKeys.ChapterUpdatedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileUpdatingTheChapter])
            };
        }

        public async Task<ApiResponse> Handle(UploadChapterImageCommand request, CancellationToken cancellationToken)
        {
            var (result, image) = await chapterService.UploadChapterImageAsync(request.Id, request.Image);
            return result switch
            {
                "ChapterNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.ChapterNotFound]),
                "FailedToDeleteOldImageFromCloudinary" => InternalServerError(stringLocalizer[SharedTranslationKeys.FailedToDeleteOldImageFromCloudinary]),
                "AnErrorOccurredWhileProcessingImageModificationRequest" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileProcessingImageModificationRequest]),
                "TheImageHasBeenChangedSuccessfully" => Success(new
                {
                    ImageUrl = image
                }, message: stringLocalizer[SharedTranslationKeys.TheImageHasBeenChangedSuccessfully]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileProcessingImageModificationRequest])
            };
        }

        public async Task<ApiResponse> Handle(UploadChapterImagesCommand request, CancellationToken cancellationToken)
        {
            var result = await chapterService.UploadChapterImagesAsync(request.Id, request.Images);
            return result switch
            {
                "ChapterNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.ChapterNotFound]),
                "AnErrorOccurredWhileProcessingTheImagesIploadRequest" =>
                InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileProcessingTheImagesIploadRequest]),
                "ImagesAreBeingUploadedToCloudStoragePleaseWaitALittleWhile" => 
                Accepted(stringLocalizer[SharedTranslationKeys.ImagesAreBeingUploadedToCloudStoragePleaseWaitALittleWhile]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileProcessingTheImagesIploadRequest])
            };
        }
    }
}
