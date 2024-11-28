using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace OnlineStore.Web.Binders
{
    public class CustomDecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // Ensure it's binding to a decimal type
            if (bindingContext.ModelMetadata.ModelType != typeof(decimal) &&
                bindingContext.ModelMetadata.ModelType != typeof(decimal?))
            {
                return Task.CompletedTask;
            }

            // Get the value from the value provider
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                // No value found
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                // No value to bind
                return Task.CompletedTask;
            }

            // Remove any spaces and standardize decimal separator
            value = value.Trim().Replace(" ", "").Replace(",", ".");

            // Attempt to parse
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedValue))
            {
                bindingContext.Result = ModelBindingResult.Success(parsedValue);
            }
            else
            {
                // Parsing failed
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName, "Invalid decimal format.");
            }

            return Task.CompletedTask;
        }
    }
}
