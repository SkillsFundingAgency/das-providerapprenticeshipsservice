using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes
{
    public class TrimStringModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return new BinderTypeModelBinder(typeof(TrimStringModelBinder));
        }
    }
}
