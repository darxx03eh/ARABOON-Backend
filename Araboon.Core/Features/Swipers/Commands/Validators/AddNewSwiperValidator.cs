using Araboon.Core.Features.Swipers.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Infrastructure.IRepositories;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Swipers.Commands.Validators
{
    public class AddNewSwiperValidator : AbstractValidator<AddNewSwiperCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        public AddNewSwiperValidator(IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            ApplyValidationRules();
        }
        public void ApplyValidationRules()
        {
            RuleFor(image => image.Image)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.ImageIsRequired])
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.ImageIsRequired]);

            RuleFor(image => image.Image.Length)
                .LessThanOrEqualTo(2 * 1024 * 1024).WithMessage(stringLocalizer[SharedTranslationKeys.ImageSizeMustNotExceed5MB]);

            RuleFor(image => image.Image.ContentType)
                .Must(ct => ct == "image/jpeg" || ct == "image/png" || ct == "image/webp")
                .WithMessage(stringLocalizer[SharedTranslationKeys.OnlyJPEGPNGAndWebPFormatsAreAllowed]);

            RuleFor(note => note.Note)
                .MaximumLength(500).WithMessage(stringLocalizer[SharedTranslationKeys.NoteMustNotExceed500Characters])
                .When(x => !string.IsNullOrWhiteSpace(x.Note));
        }
        
    }
}
