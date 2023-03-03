using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes
{
    public class ProviderUkPrnCheckActionFilter : IActionFilter
    {
        const string ProviderIdKey = "providerId";

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionArguments.ContainsKey(ProviderIdKey))
            {
                var providerIdFromAction = filterContext.ActionArguments[ProviderIdKey];

                if ($"{providerIdFromAction}" == string.Empty)
                {
                    throw new HttpException((int)HttpStatusCode.BadRequest, "Missing provider id");
                }

                var claimUkprn = filterContext.HttpContext.GetClaimValue(DasClaimTypes.Ukprn);

                if ($"{providerIdFromAction}" != claimUkprn)
                {
                    throw new HttpException((int)HttpStatusCode.Forbidden,
                        $"Mismatched UKPRNs ({providerIdFromAction} requested on URL, user's claim contains {claimUkprn}");
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}
