using Araboon.Core.Features.Mangas.Commands.Models;
using Araboon.Core.Translations;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Mangas.Commands.Validators
{
    public class UpdateMangaValidator : AbstractValidator<UpdateMangaCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;

        public UpdateMangaValidator(IStringLocalizer<SharedTranslation> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            ApplyValidationRules();
        }
        public void ApplyValidationRules()
        {
            RuleFor(x => x.MangaId)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.MangaIdIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.MangaIdNotNull]);
            RuleFor(x => x.MangaNameEn)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.MangaNameEnIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.MangaNameEnNotNull])
                .MaximumLength(100).WithMessage(stringLocalizer[SharedTranslationKeys.MangaNameEnMustNotExceed100Characters]);

            RuleFor(x => x.MangaNameAr)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.MangaNameArIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.MangaNameArNotNull])
                .MaximumLength(100).WithMessage(stringLocalizer[SharedTranslationKeys.MangaNameArMustNotExceed100Characters]);

            RuleFor(x => x.StatusEn)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.StatusEnIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.StatusEnNotNull])
                .MaximumLength(50).WithMessage(stringLocalizer[SharedTranslationKeys.StatusEnMustNotExceed50Characters])
                .Must(status => status == "Ongoing" || status == "Completed" || status == "One Shot")
                .WithMessage(stringLocalizer[SharedTranslationKeys.StatusInEnglishMustBeEitherOngoingOrCompletedOrOneShot]);

            RuleFor(x => x.StatusAr)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.StatusArIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.StatusArNotNull])
                .MaximumLength(50).WithMessage(stringLocalizer[SharedTranslationKeys.StatusArMustNotExceed50Characters])
                .Must(status => status == "مستمر" || status == "مكتمل" || status == "ون شوت")
                .WithMessage(stringLocalizer[SharedTranslationKeys.StatusInArabicMustBeEitherOngoingOrCompletedOrOneShot]);

            RuleFor(x => x.TypeEn)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.TypeEnIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.TypeEnNotNull])
                .MaximumLength(50).WithMessage(stringLocalizer[SharedTranslationKeys.TypeEnMustNotExceed50Characters])
                .Must(type => type == "Manga" || type == "Manhwa" || type == "Manhua")
                .WithMessage(stringLocalizer[SharedTranslationKeys.TypeInEnglishMustBeEitherMangaOrManhwaOrManhua]);

            RuleFor(x => x.TypeAr)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.TypeArIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.TypeArNotNull])
                .MaximumLength(50).WithMessage(stringLocalizer[SharedTranslationKeys.TypeArMustNotExceed50Characters])
                .Must(type => type == "مانجا" || type == "مانهوا" || type == "مانهوا صينية")
                .WithMessage(stringLocalizer[SharedTranslationKeys.TypeInArabicMustBeEitherMangaOrManhwaOrManhua]);

            RuleFor(x => x.AuthorEn)
                .MaximumLength(100).WithMessage(stringLocalizer[SharedTranslationKeys.AuthorEnMustNotExceed100Characters])
                .When(x => !string.IsNullOrEmpty(x.AuthorEn));

            RuleFor(x => x.AuthorAr)
                .MaximumLength(100).WithMessage(stringLocalizer[SharedTranslationKeys.AuthorArMustNotExceed100Characters])
                .When(x => !string.IsNullOrEmpty(x.AuthorAr));

            RuleFor(x => x.DescriptionEn)
                .MaximumLength(500).WithMessage(stringLocalizer[SharedTranslationKeys.DescriptionEnMustNotExceed1000Characters])
                .When(x => !string.IsNullOrEmpty(x.DescriptionEn));

            RuleFor(x => x.DescriptionAr)
                .MaximumLength(500).WithMessage(stringLocalizer[SharedTranslationKeys.DescriptionArMustNotExceed1000Characters])
                .When(x => !string.IsNullOrEmpty(x.DescriptionAr));
        }
    }
}
