using Araboon.Core.Features.Users.Commands.Models;
using Araboon.Core.Translations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Users.Commands.Validators
{
    public class ChangeCroppedCoverImageValidator : AbstractValidator<ChangeCroppedCoverImageCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        public ChangeCroppedCoverImageValidator(IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            ApplyValidationRules();
        }
        private void ApplyValidationRules()
        {
            RuleFor(image => image.Image)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.CroppedImageIsRequired]);
            RuleFor(image => image.Image.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024).WithMessage(stringLocalizer[SharedTranslationKeys.CroppedImageSizeMustNotExceed5MB]);
            RuleFor(image => image.Image.ContentType)
                .Must(ct => ct == "image/jpeg" || ct == "image/png" || ct == "image/webp")
                .WithMessage(stringLocalizer[SharedTranslationKeys.CroppedImageOnlyJPEGPNGAndWebPFormatsAreAllowed]);
        }
    }
}
