using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Araboon.Data.Helpers
{
    public class DecimalModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(decimal) || context.Metadata.ModelType == typeof(decimal?))
                return new DecimalModelBinder();
            return null;
        }
    }
}
