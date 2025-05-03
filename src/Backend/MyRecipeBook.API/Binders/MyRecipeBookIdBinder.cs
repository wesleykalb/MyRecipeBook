using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sqids;

namespace MyRecipeBook.API.Binders;

public class MyRecipeBookIdBinder : IModelBinder
{
    private readonly SqidsEncoder<long> _sqidsEncoder;

    public MyRecipeBookIdBinder(SqidsEncoder<long> sqidsEncoder)
    {
        _sqidsEncoder = sqidsEncoder;
    }
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var modelName = bindingContext.ModelName;
        var valueProvider = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProvider == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProvider);

        var value = valueProvider.FirstValue;

        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        var id = _sqidsEncoder.Decode(value).Single();

        bindingContext.Result = ModelBindingResult.Success(id);
        
        return Task.CompletedTask;
    }
}