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
            var (result, id, imageUrl) = await mangaService.AddNewMangaCommandAsync(request);
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
                }, message: SharedTranslationKeys.MangaAddedSuccessfully),
                "AnErrorOccurredWhileAddingTheManga" => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingTheManga]),
                _ => InternalServerError(stringLocalizer[SharedTranslationKeys.AnErrorOccurredWhileAddingTheManga])
            };
        }
    }
}
