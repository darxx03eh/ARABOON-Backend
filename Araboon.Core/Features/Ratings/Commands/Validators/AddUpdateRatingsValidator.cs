using Araboon.Core.Features.Ratings.Commands.Models;
using Araboon.Core.Translations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Ratings.Commands.Validators
{
    public class AddUpdateRatingsValidator : AbstractValidator<AddUpdateRatingsCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public AddUpdateRatingsValidator(IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            ApplyValidationRules();
        }
        private void ApplyValidationRules()
        {
            RuleFor(rate => rate.Rate)
                .LessThanOrEqualTo(5).WithMessage(stringLocalizer[SharedTranslationKeys.RateShouldBeLessThanOrEqualToFive])
                .GreaterThanOrEqualTo(0).WithMessage(stringLocalizer[SharedTranslationKeys.RateShouldBeGreaterThanOrEqualToZero]);        }
    }
}
