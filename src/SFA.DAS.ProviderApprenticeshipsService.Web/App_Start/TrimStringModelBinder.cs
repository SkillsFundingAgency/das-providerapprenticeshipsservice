using System.Web.Mvc;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.App_Start
{
    public sealed class TrimStringModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueResult == null || valueResult.AttemptedValue == null)
            {
                return null;
            }
            else if (valueResult.AttemptedValue == string.Empty)
            {
                return string.Empty;
            }

            return valueResult.AttemptedValue.Trim();
        }
    }
}