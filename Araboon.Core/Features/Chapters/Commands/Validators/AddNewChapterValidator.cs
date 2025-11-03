using Araboon.Core.Features.Chapters.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Infrastructure.IRepositories;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Chapters.Commands.Validators
{
    public class AddNewChapterValidator : AbstractValidator<AddNewChapterCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IUnitOfWork unitOfWork;

        public AddNewChapterValidator(IStringLocalizer<SharedTranslation> stringLocalizer, IUnitOfWork unitOfWork)
        {
            this.stringLocalizer = stringLocalizer;
            this.unitOfWork = unitOfWork;
            ApplyValidationRules();
            ApplyCustomValidationRules();
        }
        public void ApplyValidationRules()
        {
            RuleFor(x => x.MangaId)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.MangaIdIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.MangaIdIsRequired])
                .GreaterThan(0).WithMessage(stringLocalizer[SharedTranslationKeys.MangaIdGreaterThanZero]);

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

            RuleFor(image => image.Image.Length)
                .LessThanOrEqualTo(2 * 1024 * 1024).WithMessage(stringLocalizer[SharedTranslationKeys.ImageSizeMustNotExceed5MB])
                .When(x => x.Image is not null);

            RuleFor(image => image.Image.ContentType)
                .Must(ct => ct == "image/jpeg" || ct == "image/png" || ct == "image/webp")
                .WithMessage(stringLocalizer[SharedTranslationKeys.OnlyJPEGPNGAndWebPFormatsAreAllowed])
                .When(x => x.Image is not null);

            RuleFor(image => image.ChapterImages)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.ChapterImagesAreRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.ChapterImagesAreRequired]);

            RuleForEach(x => x.ChapterImages).ChildRules(image =>
            {
                image.RuleFor(file => file.Length)
                     .LessThanOrEqualTo(2 * 1024 * 1024).WithMessage(stringLocalizer[SharedTranslationKeys.ChapterImageSizeMustNotExceed5MB]);

                image.RuleFor(file => file.ContentType)
                     .Must(ct => ct == "image/jpeg" || ct == "image/png" || ct == "image/webp")
                     .WithMessage(stringLocalizer[SharedTranslationKeys.OnlyJPEGPNGAndWebPFormatsAreAllowed]);
            });
        }
        private void ApplyCustomValidationRules()
        {
            RuleFor(x => x.ChapterNo)
                .MustAsync(async (obj, key, cancellation) =>
                {
                    var exist = await unitOfWork.ChapterRepository.isChapterNoExistAsync(
                        obj.MangaId, key, obj.Language
                    );
                    return !exist;
                }).WithMessage(stringLocalizer[SharedTranslationKeys.ChapterNoForThisLanguageAlreadyExist]);
        }
    }
}
