using Araboon.Core.Features.Authentications.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Data.Entities.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Authentications.Commands.Validators
{
    public class RegistrationUserValidator : AbstractValidator<RegistrationUserCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly UserManager<AraboonUser> userManager;

        public RegistrationUserValidator(IStringLocalizer<SharedTranslation> stringLocalizer, UserManager<AraboonUser> userManager)
        {
            this.stringLocalizer = stringLocalizer;
            this.userManager = userManager;
            ApplyValidationRules();
            ApplyCustomValidationRules();
        }
        private void ApplyValidationRules()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.FirstNameNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.FirstNameNotNull]);
            RuleFor(user => user.LastName)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.LastNameNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.LastNameNotNull]);
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.EmailNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.EmailNotNull])
                .EmailAddress().WithMessage(stringLocalizer[SharedTranslationKeys.NotValidEmail]);
            RuleFor(user => user.UserName)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.UserNameNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.UserNameNotNull]);
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
            RuleFor(user => user.UserName)
                .MustAsync(async (key, cancellation) =>
                {
                    var username = await userManager.FindByNameAsync(key);
                    return username is null;
                }).WithMessage(stringLocalizer[SharedTranslationKeys.UserNameAlreadyExist]);
            RuleFor(user => user.Email)
                .MustAsync(async (key, cancellation) =>
                {
                    var email = await userManager.FindByEmailAsync(key);
                    return email is null;
                }).WithMessage(stringLocalizer[SharedTranslationKeys.EmailAlreadyExist]);
        }
    }
}
