using Araboon.Core.Features.Categories.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Infrastructure.IRepositories;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Categories.Commands.Validators
{
    public class AddNewCategoryValidator : AbstractValidator<AddNewCategoryCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IUnitOfWork unitOfWork;

        public AddNewCategoryValidator(IStringLocalizer<SharedTranslation> stringLocalizer, IUnitOfWork unitOfWork)
        {
            this.stringLocalizer = stringLocalizer;
            this.unitOfWork = unitOfWork;
            ApplyValidationRules();
            ApplyCustomValidationRules();
        }
        private void ApplyValidationRules()
        {
            RuleFor(x => x.CategoryNameEn)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.CategoryNameEnIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.CategoryNameEnNotNull])
                .MaximumLength(100).WithMessage(stringLocalizer[SharedTranslationKeys.CategoryNameEnMaxLengthIs100]);

            RuleFor(x => x.CategoryNameAr)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.CategoryNameArIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.CategoryNameArNotNull])
                .MaximumLength(100).WithMessage(stringLocalizer[SharedTranslationKeys.CategoryNameArMaxLengthIs100]);

        }
        private void ApplyCustomValidationRules()
        {
            RuleFor(x => x.CategoryNameEn)
                .MustAsync(async (key, cancellation) =>
                {
                    var exist = await unitOfWork.CategoryRepository.IsCategoryNameEnExist(key);
                    return !exist;
                }).WithMessage(stringLocalizer[SharedTranslationKeys.CategoryNameEnAlreadyExist]);
            RuleFor(x => x.CategoryNameAr)
                .MustAsync(async (key, cancellation) =>
                {
                    var exist = await unitOfWork.CategoryRepository.IsCategoryNameArExist(key);
                    return !exist;
                }).WithMessage(stringLocalizer[SharedTranslationKeys.CategoryNameArAlreadyExist]);
        }
    }
}
