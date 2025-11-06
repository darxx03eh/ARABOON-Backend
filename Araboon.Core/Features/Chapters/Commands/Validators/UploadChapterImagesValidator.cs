using Araboon.Core.Features.Chapters.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Infrastructure.IRepositories;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Chapters.Commands.Validators
{
    public class UploadChapterImagesValidator : AbstractValidator<UploadChapterImagesCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IUnitOfWork unitOfWork;

        public UploadChapterImagesValidator(IStringLocalizer<SharedTranslation> stringLocalizer, IUnitOfWork unitOfWork)
        {
            this.stringLocalizer = stringLocalizer;
            this.unitOfWork = unitOfWork;
            ApplyValidationRules();
        }
        public void ApplyValidationRules()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.ChapterIdIsRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.ChapterIdIsRequired])
                .GreaterThan(0).WithMessage(stringLocalizer[SharedTranslationKeys.ChapterIdGreaterThanZero]);

            RuleFor(image => image.Images)
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.ChapterImagesAreRequired])
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.ChapterImagesAreRequired]);

            RuleForEach(x => x.Images).ChildRules(image =>
            {
                image.RuleFor(file => file.Length)
                     .LessThanOrEqualTo(2 * 1024 * 1024).WithMessage(stringLocalizer[SharedTranslationKeys.ChapterImageSizeMustNotExceed5MB]);

                image.RuleFor(file => file.ContentType)
                     .Must(ct => ct == "image/jpeg" || ct == "image/png" || ct == "image/webp")
                     .WithMessage(stringLocalizer[SharedTranslationKeys.OnlyJPEGPNGAndWebPFormatsAreAllowed]);
            });
        }
    }
}
