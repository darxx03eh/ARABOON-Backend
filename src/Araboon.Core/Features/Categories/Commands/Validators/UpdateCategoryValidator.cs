using Araboon.Core.Features.Categories.Commands.Models;
using Araboon.Core.Translations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Categories.Commands.Validators
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        public UpdateCategoryValidator(IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            ApplyValidationRules();
        }
        private void ApplyValidationRules()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.CategoryIdIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.CategoryIdNotNull]);

            RuleFor(x => x.CategoryNameEn)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.CategoryNameEnIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.CategoryNameEnNotNull])
                .MaximumLength(100).WithMessage(stringLocalizer[SharedTranslationKeys.CategoryNameEnMaxLengthIs100]);

            RuleFor(x => x.CategoryNameAr)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.CategoryNameArIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.CategoryNameArNotNull])
                .MaximumLength(100).WithMessage(stringLocalizer[SharedTranslationKeys.CategoryNameArMaxLengthIs100]);

        }
    }
}
