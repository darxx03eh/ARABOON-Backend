using Araboon.Core.Features.Swipers.Commands.Models;
using Araboon.Core.Translations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Swipers.Commands.Validators
{
    public class UploadNewSwiperImageValidator : AbstractValidator<UploadNewSwiperImageCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public UploadNewSwiperImageValidator(IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            ApplyValidationRules();
        }

        public void ApplyValidationRules()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.SwiperIdIsRequired])
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.SwiperIdIsRequired])
                .GreaterThan(0).WithMessage(stringLocalizer[SharedTranslationKeys.SwiperIdMustBeGreaterThanZero]);

            RuleFor(image => image.Image)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.ImageIsRequired])
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.ImageIsRequired]);

            RuleFor(image => image.Image.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024).WithMessage(stringLocalizer[SharedTranslationKeys.ImageSizeMustNotExceed5MB]);

            RuleFor(image => image.Image.ContentType)
                .Must(ct => ct == "image/jpeg" || ct == "image/png" || ct == "image/webp")
                .WithMessage(stringLocalizer[SharedTranslationKeys.OnlyJPEGPNGAndWebPFormatsAreAllowed]);
        }
    }
}