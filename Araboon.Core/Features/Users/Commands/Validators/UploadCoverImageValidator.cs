using Araboon.Core.Features.Users.Commands.Models;
using Araboon.Core.Translations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Users.Commands.Validators
{
    public class UploadCoverImageValidator : AbstractValidator<UploadCoverImageCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public UploadCoverImageValidator(IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            ApplyValidationRules();
        }

        private void ApplyValidationRules()
        {
            RuleFor(image => image.OriginalImage)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.ImageIsRequired]);
            RuleFor(image => image.OriginalImage.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024).WithMessage(stringLocalizer[SharedTranslationKeys.ImageSizeMustNotExceed5MB]);
            RuleFor(image => image.OriginalImage.ContentType)
                .Must(ct => ct == "image/jpeg" || ct == "image/png" || ct == "image/webp")
                .WithMessage(stringLocalizer[SharedTranslationKeys.OnlyJPEGPNGAndWebPFormatsAreAllowed]);

            RuleFor(image => image.CroppedImage)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.CroppedImageIsRequired]);
            RuleFor(image => image.CroppedImage.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024).WithMessage(stringLocalizer[SharedTranslationKeys.CroppedImageSizeMustNotExceed5MB]);
            RuleFor(image => image.CroppedImage.ContentType)
                .Must(ct => ct == "image/jpeg" || ct == "image/png" || ct == "image/webp")
                .WithMessage(stringLocalizer[SharedTranslationKeys.CroppedImageOnlyJPEGPNGAndWebPFormatsAreAllowed]);
        }
    }
}
