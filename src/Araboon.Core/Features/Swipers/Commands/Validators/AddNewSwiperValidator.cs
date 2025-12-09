using Araboon.Core.Features.Swipers.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Infrastructure.IRepositories;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Swipers.Commands.Validators
{
    public class AddNewSwiperValidator : AbstractValidator<AddNewSwiperCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IUnitOfWork unitOfWork;

        public AddNewSwiperValidator(IStringLocalizer<SharedTranslation> stringLocalizer, IUnitOfWork unitOfWork)
        {
            this.stringLocalizer = stringLocalizer;
            this.unitOfWork = unitOfWork;
            ApplyValidationRules();
            ApplyCustomValidationRules();
        }

        public void ApplyValidationRules()
        {
            RuleFor(image => image.Image)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.ImageIsRequired])
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.ImageIsRequired]);

            RuleFor(image => image.Image.Length)
                .LessThanOrEqualTo(5 * 1024 * 1024).WithMessage(stringLocalizer[SharedTranslationKeys.ImageSizeMustNotExceed5MB]);

            RuleFor(image => image.Image.ContentType)
                .Must(ct => ct == "image/jpeg" || ct == "image/png" || ct == "image/webp")
                .WithMessage(stringLocalizer[SharedTranslationKeys.OnlyJPEGPNGAndWebPFormatsAreAllowed]);

            RuleFor(note => note.NoteEn)
                .MaximumLength(500).WithMessage(stringLocalizer[SharedTranslationKeys.NoteEnMustNotExceed500Characters])
                .When(x => !string.IsNullOrWhiteSpace(x.NoteEn));

            RuleFor(note => note.NoteAr)
                .MaximumLength(500).WithMessage(stringLocalizer[SharedTranslationKeys.NoteArMustNotExceed500Characters])
                .When(x => !string.IsNullOrWhiteSpace(x.NoteAr));

            RuleFor(link => link.Link)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.LinkIsRequired])
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.LinkIsRequired])
                .Must(link => link.ToLower().StartsWith("https")).WithMessage(stringLocalizer[SharedTranslationKeys.LinkMustStartsWithHTTPS]);
        }

        private void ApplyCustomValidationRules()
        {
            RuleFor(x => x.Link)
                .MustAsync(async (obj, key, cancellation) =>
                {
                    var exist = await unitOfWork.SwiperRepository.IsLinkExistsAsync(key);
                    return !exist;
                }).WithMessage(stringLocalizer[SharedTranslationKeys.LinkAlreadyExist]);
        }
    }
}