namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;

public class TrimStringModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }
        if (!context.Metadata.IsComplexType && context.Metadata.ModelType == typeof(string))
        {
            return new TrimStringModelBinder(new SimpleTypeModelBinder(context.Metadata.ModelType, new LoggerFactory()));
        }
        return null;
    }
}