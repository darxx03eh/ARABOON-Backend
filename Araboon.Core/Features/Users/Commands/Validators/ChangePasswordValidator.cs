using Araboon.Core.Features.Users.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Data.Entities.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Users.Commands.Validators
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        public ChangePasswordValidator(IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            ApplyValidationRules();
        }
        private void ApplyValidationRules()
        {
            RuleFor(user => user.CurrentPassword)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.CurrentPasswordNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.CurrentPasswordNotNull])
                .NotEqual(user => user.NewPassword).WithMessage(stringLocalizer[SharedTranslationKeys.TheNewPasswordMustBeDifferentFromTheCurrentOne]);
            RuleFor(user => user.NewPassword)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.PasswordNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.PasswordNotNull])
                .MinimumLength(8).WithMessage(stringLocalizer[SharedTranslationKeys.PasswordMinimumLength]);
            RuleFor(user => user.ConfirmNewPassword)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.ConfirmPasswordNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.ConfirmPasswordNotNull])
                .MinimumLength(8).WithMessage(stringLocalizer[SharedTranslationKeys.ConfirmPasswordMinimumLength])
                .Equal(user => user.NewPassword).WithMessage(stringLocalizer[SharedTranslationKeys.ConfirmPasswordMustEqualToPassword]);
        }
    }
}
