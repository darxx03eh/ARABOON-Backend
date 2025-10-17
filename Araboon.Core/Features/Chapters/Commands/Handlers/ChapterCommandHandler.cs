using Araboon.Core.Bases;
using Araboon.Core.Features.Chapters.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Chapters.Commands.Handlers
{
    public class ChapterCommandHandler : ApiResponseHandler
        , IRequestHandler<ChapterReadCommand, ApiResponse>
    {
        private readonly IChapterService chapterService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public ChapterCommandHandler(IChapterService chapterService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.chapterService = chapterService;
            this.stringLocalizer = stringLocalizer;
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
    }
}
