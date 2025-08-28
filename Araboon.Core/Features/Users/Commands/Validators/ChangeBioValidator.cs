using Araboon.Core.Features.Users.Commands.Models;
using Araboon.Core.Translations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Users.Commands.Validators
{
    public class ChangeBioValidator : AbstractValidator<ChangeBioCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public ChangeBioValidator(IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            ApplyValidationRules();
        }
        private void ApplyValidationRules()
        {
            RuleFor(image => image.Bio)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.BioIsRequired]);
            RuleFor(image => image.Bio.Length)
                .LessThanOrEqualTo(150).WithMessage(stringLocalizer[SharedTranslationKeys.BioSizeMustNotExceed150Characters]);
        }
    }
}
