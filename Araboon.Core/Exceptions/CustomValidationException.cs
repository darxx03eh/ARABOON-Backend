using Araboon.Core.Translations;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;

namespace Araboon.Core.Exceptions
{
    public class CustomValidationException : Exception
    {
        private readonly IStringLocalizer<SharedTranslation> stringLocalizer;
        public Dictionary<String, List<String>> Errors { get; set; }
        public CustomValidationException(IEnumerable<ValidationFailure> failures,
                                         IStringLocalizer<SharedTranslation> stringLocalizer)
            : base(stringLocalizer[SharedTranslationKeys.ValidationFailed])
        {
            Errors = failures.GroupBy(fail => fail.PropertyName)
            .ToDictionary(map => map.Key, map => map.Select(fail => fail.ErrorMessage).ToList());
            this.stringLocalizer = stringLocalizer;
        }
    }
}
