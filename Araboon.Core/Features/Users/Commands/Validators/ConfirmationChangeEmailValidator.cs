using Araboon.Core.Features.Users.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Data.Entities.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Users.Commands.Validators
{
    public class ConfirmationChangeEmailValidator : AbstractValidator<ConfirmationChangeEmailCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly UserManager<AraboonUser> userManager;

        public ConfirmationChangeEmailValidator(IStringLocalizer<SharedTranslation> stringLocalizer, UserManager<AraboonUser> userManager)
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
            RuleFor(user => user.UserId)
                .MustAsync(async (key, cancellation) =>
                {
                    var user = await userManager.FindByIdAsync(key);
                    return user is not null;
                }).WithMessage(stringLocalizer[SharedTranslationKeys.UserNotFound]);
        }
    }
}
