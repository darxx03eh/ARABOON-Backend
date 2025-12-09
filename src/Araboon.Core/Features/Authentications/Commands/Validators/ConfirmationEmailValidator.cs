using Araboon.Core.Features.Authentications.Commands.Models;
using Araboon.Core.Translations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Authentications.Commands.Validators
{
    public class ConfirmationEmailValidator : AbstractValidator<ConfirmationEmailCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        public ConfirmationEmailValidator(IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            ApplyValidationRules();
        }
        private void ApplyValidationRules()
        {
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.EmailNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.EmailNotNull])
                .EmailAddress().WithMessage(stringLocalizer[SharedTranslationKeys.NotValidEmail]);
        }
    }
}
