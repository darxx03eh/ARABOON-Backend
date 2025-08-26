using Araboon.Core.Features.Users.Commands.Models;
using Araboon.Core.Translations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Users.Commands.Validators
{
    public class UploadProfileImageValidator : AbstractValidator<UploadProfileImageCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public UploadProfileImageValidator(IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            ApplyValidationRules();
        }

        private void ApplyValidationRules()
        {
            RuleFor(image => image.ProfileImage)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.ImageIsRequired]);
            RuleFor(image => image.ProfileImage.Length)
                .LessThanOrEqualTo(2 * 1024 * 1024).WithMessage(stringLocalizer[SharedTranslationKeys.ImageSizeMustNotExceed2MB]);
            RuleFor(image => image.ProfileImage.ContentType)
                .Must(ct => ct == "image/jpeg" || ct == "image/png" || ct == "image/webp")
                .WithMessage(stringLocalizer[SharedTranslationKeys.OnlyJPEGPNGAndWebPFormatsAreAllowed]);
        }
    }
}
