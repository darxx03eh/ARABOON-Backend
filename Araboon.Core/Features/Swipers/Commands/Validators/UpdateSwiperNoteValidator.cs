using Araboon.Core.Features.Swipers.Commands.Models;
using Araboon.Core.Translations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Swipers.Commands.Validators
{
    public class UpdateSwiperNoteValidator : AbstractValidator<UpdateSwiperNoteCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        public UpdateSwiperNoteValidator(IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            ApplyValidationRules();
        }
        public void ApplyValidationRules()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.SwiperIdIsRequired])
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.SwiperIdIsRequired])
                .GreaterThan(0).WithMessage(stringLocalizer[SharedTranslationKeys.SwiperIdMustBeGreaterThanZero]);

            RuleFor(note => note.Note)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.SwiperNoteIsRequired])
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.SwiperNoteIsRequired])
                .MaximumLength(500).WithMessage(stringLocalizer[SharedTranslationKeys.NoteMustNotExceed500Characters]);
        }
    }
}
