using Araboon.Core.Features.Authentications.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Data.Entities.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Authentications.Commands.Validators
{
    public class SignInValidator : AbstractValidator<SignInCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly UserManager<AraboonUser> userManager;

        public SignInValidator(IStringLocalizer<SharedTranslation> stringLocalizer, UserManager<AraboonUser> userManager)
        {
            this.stringLocalizer = stringLocalizer;
            this.userManager = userManager;
            ApplyValidationRules();
        }
        private void ApplyValidationRules()
        {
            RuleFor(user => user.UserName)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.UserNameNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.UserNameNotNull]);
            RuleFor(user => user.Password)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.PasswordNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.PasswordNotNull])
                .MinimumLength(6).WithMessage(stringLocalizer[SharedTranslationKeys.PasswordMinimumLength]);
        }
    }
}
