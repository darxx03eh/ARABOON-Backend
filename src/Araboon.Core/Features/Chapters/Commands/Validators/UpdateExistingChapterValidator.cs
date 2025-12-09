using Araboon.Core.Features.Chapters.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Infrastructure.IRepositories;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Chapters.Commands.Validators
{
    public class UpdateExistingChapterValidator : AbstractValidator<UpdateExistingChapterCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IUnitOfWork unitOfWork;

        public UpdateExistingChapterValidator(IStringLocalizer<SharedTranslation> stringLocalizer, IUnitOfWork unitOfWork)
        {
            this.stringLocalizer = stringLocalizer;
            this.unitOfWork = unitOfWork;
            ApplyValidationRules();
            ApplyCustomValidationRules();
        }
        public void ApplyValidationRules()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.ChapterIdIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.ChapterIdIsRequired])
                .GreaterThan(0).WithMessage(stringLocalizer[SharedTranslationKeys.ChapterIdGreaterThanZero]);

            RuleFor(x => x.ChapterNo)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.ChapterNoIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.ChapterNoNotNull])
                .GreaterThan(0).WithMessage(stringLocalizer[SharedTranslationKeys.ChapterNoGreaterThanZero]);

            RuleFor(x => x.Language)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.LanguageIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.LanguageNotNull])
                .MaximumLength(50).WithMessage(stringLocalizer[SharedTranslationKeys.LanguageMustNotExceed50Characters])
                .Must(lang => lang == "Arabic" || lang == "English")
                .WithMessage(stringLocalizer[SharedTranslationKeys.LanguageMustBeEitherArabicOrEnglish]);


            RuleFor(x => x.EnglishChapterTitle)
                .MaximumLength(100).WithMessage(stringLocalizer[SharedTranslationKeys.EnglishChapterTitleMustNotExceed100Characters])
                .When(x => !string.IsNullOrEmpty(x.EnglishChapterTitle));

            RuleFor(x => x.ArabicChapterTitle)
                .MaximumLength(100).WithMessage(stringLocalizer[SharedTranslationKeys.ArabicChapterTitleMustNotExceed100Characters])
                .When(x => !string.IsNullOrEmpty(x.ArabicChapterTitle));
        }
        private void ApplyCustomValidationRules()
        {
            RuleFor(x => x.ChapterNo)
                .MustAsync(async (obj, key, cancellation) =>
                {
                    var chapter = await unitOfWork.ChapterRepository.GetByIdAsync(obj.Id);
                    if (chapter is null)
                        return true;
                    var exist = await unitOfWork.ChapterRepository.isChapterNoExistAsync(
                        chapter.MangaID, key, obj.Language, obj.Id
                    );
                    return !exist;
                }).WithMessage(stringLocalizer[SharedTranslationKeys.ChapterNoForThisLanguageAlreadyExist]);
        }
    }
}
