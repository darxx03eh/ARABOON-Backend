using Araboon.Core.Features.Authentications.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Data.Entities.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Authentications.Commands.Validators
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly UserManager<AraboonUser> userManager;

        public ResetPasswordValidator(IStringLocalizer<SharedTranslation> stringLocalizer, UserManager<AraboonUser> userManager)
        {
            this.stringLocalizer = stringLocalizer;
            this.userManager = userManager;
            ApplyValidationRules();
            ApplyCustomValidationRules();
        }
        private void ApplyValidationRules()
        {
            RuleFor(user => user.Token)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.PasswordResetTokenNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.PasswordResetTokenNotNull]);
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.EmailNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.EmailNotNull])
                .EmailAddress().WithMessage(stringLocalizer[SharedTranslationKeys.NotValidEmail]);
            RuleFor(user => user.Password)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.PasswordNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.PasswordNotNull])
                .MinimumLength(8).WithMessage(stringLocalizer[SharedTranslationKeys.PasswordMinimumLength]);
            RuleFor(user => user.ConfirmPassword)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.ConfirmPasswordNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.ConfirmPasswordNotNull])
                .MinimumLength(8).WithMessage(stringLocalizer[SharedTranslationKeys.ConfirmPasswordMinimumLength])
                .Equal(user => user.Password).WithMessage(stringLocalizer[SharedTranslationKeys.ConfirmPasswordMustEqualToPassword]);
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
