using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ModelBindingDemo.ModelBinders
{
    public class CustomDateModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
                return Task.CompletedTask;

            // Support multiple date formats
            string[] formats = {
                "yyyy-MM-dd",
                "dd/MM/yyyy",
                "MM/dd/yyyy",
                "yyyy/MM/dd",
                "dd-MM-yyyy",
                "yyyyMMdd"
            };

            DateTime? date = null;
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var parsedDate))
                {
                    date = parsedDate;
                    break;
                }
            }

            if (date.HasValue)
            {
                bindingContext.Result = ModelBindingResult.Success(date.Value);
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(
                    modelName,
                    $"Invalid date format. Supported formats: {string.Join(", ", formats)}");
            }

            return Task.CompletedTask;
        }
    }

    public class CustomDateModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(DateTime) ||
                context.Metadata.ModelType == typeof(DateTime?))
            {
                return new CustomDateModelBinder();
            }
            return null;
        }
    }
}
