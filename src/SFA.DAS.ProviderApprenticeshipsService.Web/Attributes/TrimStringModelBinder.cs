namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;

public sealed class TrimStringModelBinder : IModelBinder
{
    private readonly IModelBinder _fallbackBinder;

    public TrimStringModelBinder(IModelBinder fallbackBinder)
    {
        _fallbackBinder = fallbackBinder ?? throw new ArgumentNullException(nameof(fallbackBinder));
    }
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult != ValueProviderResult.None &&
            valueProviderResult.FirstValue is string str &&
            !string.IsNullOrEmpty(str))
        {
            bindingContext.Result = ModelBindingResult.Success(str.Trim());
            return Task.CompletedTask;
        }

        return _fallbackBinder.BindModelAsync(bindingContext);
    }
}