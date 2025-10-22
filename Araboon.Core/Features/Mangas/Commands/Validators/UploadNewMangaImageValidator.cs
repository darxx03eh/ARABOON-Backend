using Araboon.Core.Features.Mangas.Commands.Models;
using Araboon.Core.Translations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Mangas.Commands.Validators
{
    public class UploadNewMangaImageValidator : AbstractValidator<UploadNewMangaImageCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public UploadNewMangaImageValidator(IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            ApplyValidationRules();
        }
        public void ApplyValidationRules()
        {
            RuleFor(image => image.Image.Length)
                .LessThanOrEqualTo(2 * 1024 * 1024).WithMessage(stringLocalizer[SharedTranslationKeys.ImageSizeMustNotExceed2MB]);

            RuleFor(image => image.Image.ContentType)
                .Must(ct => ct == "image/jpeg" || ct == "image/png" || ct == "image/webp")
                .WithMessage(stringLocalizer[SharedTranslationKeys.OnlyJPEGPNGAndWebPFormatsAreAllowed]);
        }
    }
}
