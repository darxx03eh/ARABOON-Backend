using Araboon.Core.Features.Users.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Data.Entities.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Users.Commands.Validators
{
    public class ChangeNameValidator : AbstractValidator<ChangeNameCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public ChangeNameValidator(IStringLocalizer<SharedTranslation> stringLocalizer, UserManager<AraboonUser> userManager)
        {
            this.stringLocalizer = stringLocalizer;
            ApplyValidationRules();
        }
        private void ApplyValidationRules()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.FirstNameNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.FirstNameNotNull]);
            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.LastNameNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.LastNameNotNull]);
        }
    }
}
