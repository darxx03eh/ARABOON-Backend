using Araboon.Core.Features.Swipers.Commands.Models;
using Araboon.Core.Translations;
using Araboon.Infrastructure.IRepositories;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Features.Swipers.Commands.Validators
{
    public class UpdateSwiperNoteLinkValidator : AbstractValidator<UpdateSwiperNoteLinkCommand>
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        private readonly IUnitOfWork unitOfWork;

        public UpdateSwiperNoteLinkValidator(IStringLocalizer<SharedTranslation> stringLocalizer, IUnitOfWork unitOfWork)
        {
            this.stringLocalizer = stringLocalizer;
            this.unitOfWork = unitOfWork;
            ApplyValidationRules();
            ApplyCustomValidationRules();
        }
        public void ApplyValidationRules()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.SwiperIdIsRequired])
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.SwiperIdIsRequired])
                .GreaterThan(0).WithMessage(stringLocalizer[SharedTranslationKeys.SwiperIdMustBeGreaterThanZero]);

            RuleFor(note => note.NoteEn)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.SwiperNoteEnIsRequired])
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.SwiperNoteEnIsRequired])
                .MaximumLength(500).WithMessage(stringLocalizer[SharedTranslationKeys.NoteEnMustNotExceed500Characters]);

            RuleFor(note => note.NoteAr)
                .NotNull().WithMessage(stringLocalizer[SharedTranslationKeys.SwiperNoteArIsRequired])
                .NotEmpty().WithMessage(stringLocalizer[SharedTranslationKeys.SwiperNoteArIsRequired])
                .MaximumLength(500).WithMessage(stringLocalizer[SharedTranslationKeys.NoteArMustNotExceed500Characters]);

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
                    var swiper = await unitOfWork.SwiperRepository.GetByIdAsync(obj.Id);
                    if (swiper is null)
                        return true;
                    var exist = await unitOfWork.SwiperRepository.IsLinkExistsAsync(key, obj.Id);
                    return !exist;
                }).WithMessage(stringLocalizer[SharedTranslationKeys.LinkAlreadyExist]);
        }
    }
}
