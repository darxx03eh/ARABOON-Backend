using Araboon.Core.Bases;
using Araboon.Core.Features.ChapterViews.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Service.Implementations;
using Araboon.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.ChapterViews.Commands.Handlers
{
    public class ChapterViewCommandHandler : ApiResponseHandler
        , IRequestHandler<MarkAsReadCommand, ApiResponse>
        , IRequestHandler<MarkAsUnReadCommand, ApiResponse>
    {
        private readonly IChapterViewService chapterViewService;
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public ChapterViewCommandHandler(IChapterViewService chapterViewService, IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.chapterViewService = chapterViewService;
            this.stringLocalizer = stringLocalizer;
        }
        public async Task<ApiResponse> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            var result = await chapterViewService.MarkAsReadAsync(request.MangaID, request.ChapterID);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "MarkAsReadForChaptersServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.MarkAsReadForChaptersServiceforRegisteredUsersOnly]),
                "ThisChapterInThisMangaIsAlreadyMarkedAsRead" =>
                Conflict(stringLocalizer[SharedTranslationKeys.ThisChapterInThisMangaIsAlreadyMarkedAsRead]),
                "ThisChapterIsNotInThisManga" => BadRequest(stringLocalizer[SharedTranslationKeys.ThisChapterIsNotInThisManga]),
                "MarkedAsRead" => Success(null, message: stringLocalizer[SharedTranslationKeys.MarkedAsRead]),
                "ThereWasAProblemMarkedAsRead"
                => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemMarkedAsRead]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemMarkedAsRead])
            };
        }

        public async Task<ApiResponse> Handle(MarkAsUnReadCommand request, CancellationToken cancellationToken)
        {
            var result = await chapterViewService.MarkAsUnReadAsync(request.MangaID, request.ChapterID);
            return result switch
            {
                "MangaNotFound" => NotFound(stringLocalizer[SharedTranslationKeys.MangaNotFound]),
                "MarkAsUnReadForChaptersServiceforRegisteredUsersOnly" =>
                Unauthorized(stringLocalizer[SharedTranslationKeys.MarkAsUnReadForChaptersServiceforRegisteredUsersOnly]),
                "ThisChapterForThisMangaIsNotExistInMarkedAsRead" =>
                NotFound(stringLocalizer[SharedTranslationKeys.ThisChapterForThisMangaIsNotExistInMarkedAsRead]),
                "MarkedAsUnRead" => Success(null, message: stringLocalizer[SharedTranslationKeys.MarkedAsUnRead]),
                "ThereWasAProblemMarkedAsUnRead"
                => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemMarkedAsUnRead]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.ThereWasAProblemMarkedAsUnRead])
            };
        }
    }
}
