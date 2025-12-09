using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace Araboon.Data.Helpers
{
    public class DecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
            string value = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(value))
                return Task.CompletedTask;
            value = value.Replace(',', '.');
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                bindingContext.Result = ModelBindingResult.Success(result);
            else bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid decimal value");
            return Task.CompletedTask;
        }
    }
}
