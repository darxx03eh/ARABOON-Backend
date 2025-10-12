using Araboon.Core.Bases;
using Araboon.Core.Features.ChapterImages.Queries.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.ChapterImages.Queries.Handlers
{
    public class ChapterImagesQueryHandler : ApiResponseHandler
        , IRequestHandler<GetChapterImagesQuery, ApiResponse>
    {
        private readonly IChapterImagesService chapterPhotoService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public ChapterImagesQueryHandler(IChapterImagesService chapterPhotoService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.chapterPhotoService = chapterPhotoService;
            this.stringLocalizer = stringLocalizer;
        }

        public async Task<ApiResponse> Handle(GetChapterImagesQuery request, CancellationToken cancellationToken)
        {
            var (result, images) = await chapterPhotoService.GetChapterImagesAsync(request.MangaId, request.ChapterNo, request.Language);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "ChapterNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.ChapterNotFound]),
                "ChapterForArabicLanguageNotExist" => BadRequest(stringLocalizer[SharedTranslationKeys.ChapterForArabicLanguageNotExist]),
                "ChapterForEnglishLanguageNotExist" => BadRequest(stringLocalizer[SharedTranslationKeys.ChapterForEnglishLanguageNotExist]),
                "ImagesNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.ImagesNotFound]),
                "ImagesFound" => Success(images, meta: new
                {
                    imagesCount = images.Images.Count()
                }, message: stringLocalizer[SharedTranslationKeys.ImagesFound]),
                _ => NotFound(stringLocalizer[SharedTranslationKeys.ImagesNotFound])
            };
        }
    }
}
