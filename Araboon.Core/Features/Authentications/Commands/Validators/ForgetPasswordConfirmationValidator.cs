using Araboon.Core.Features.Authentications.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Data.Entities.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Authentications.Commands.Validators
{
    public class ForgetPasswordConfirmationValidator : AbstractValidator<ForgetPasswordConfirmationCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly UserManager<AraboonUser> userManager;

        public ForgetPasswordConfirmationValidator(IStringLocalizer<SharedTranslation> stringLocalizer, UserManager<AraboonUser> userManager)
        {
            this.stringLocalizer = stringLocalizer;
            this.userManager = userManager;
            ApplyValidationRules();
            ApplyCustomValidationRules();
        }
        private void ApplyValidationRules()
        {
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.EmailNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.EmailNotNull])
                .EmailAddress().WithMessage(stringLocalizer[SharedTranslationKeys.NotValidEmail]);
        }
        private void ApplyCustomValidationRules()
        {
            RuleFor(user => user.Email)
                .MustAsync(async (key, cancellation) =>
                {
                    var email = await userManager.FindByEmailAsync(key);
                    return email is not null;
                }).WithMessage(stringLocalizer[SharedTranslationKeys.UserNotFound]);
        }
    }
}
