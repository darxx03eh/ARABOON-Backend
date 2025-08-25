using Araboon.Core.Features.Users.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Data.Entities.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Users.Commands.Validators
{
    public class ChangeUserNameValidator : AbstractValidator<ChangeUserNameCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly UserManager<AraboonUser> userManager;

        public ChangeUserNameValidator(IStringLocalizer<SharedTranslation> stringLocalizer, UserManager<AraboonUser> userManager)
        {
            this.stringLocalizer = stringLocalizer;
            this.userManager = userManager;
            ApplyValidationRules();
            ApplyCustomValidationRules();
        }
        private void ApplyValidationRules()
        {
            RuleFor(user => user.UserName)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.UserNameNotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.UserNameNotNull]);
        }
        private void ApplyCustomValidationRules()
        {
            RuleFor(user => user.UserName)
                .MustAsync(async (key, cancellation) =>
                {
                    var username = await userManager.FindByNameAsync(key);
                    return username is null;
                }).WithMessage(stringLocalizer[SharedTranslationKeys.UserNameAlreadyExist]);
        }
    }
}
