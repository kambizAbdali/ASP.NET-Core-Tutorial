using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ModelBindingDemo.ModelBinders
{
    public class CsvArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(string[]) &&
                bindingContext.ModelType != typeof(List<string>))
            {
                return Task.CompletedTask;
            }

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                if (bindingContext.ModelType == typeof(string[]))
                    bindingContext.Result = ModelBindingResult.Success(Array.Empty<string>());
                else
                    bindingContext.Result = ModelBindingResult.Success(new List<string>());
                return Task.CompletedTask;
            }

            // Convert CSV string to array
            var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(v => v.Trim())
                             .Where(v => !string.IsNullOrEmpty(v))
                             .ToArray();

            if (bindingContext.ModelType == typeof(List<string>))
                bindingContext.Result = ModelBindingResult.Success(values.ToList());
            else
                bindingContext.Result = ModelBindingResult.Success(values);

            return Task.CompletedTask;
        }
    }

    public class CsvArrayModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(string[]) ||
                context.Metadata.ModelType == typeof(List<string>))
            {
                return new CsvArrayModelBinder();
            }
            return null;
        }
    }
}
