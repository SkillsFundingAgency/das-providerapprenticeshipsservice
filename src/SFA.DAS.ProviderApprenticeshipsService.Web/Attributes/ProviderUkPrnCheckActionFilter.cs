using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Azure.Documents.SystemFunctions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes
{
    public class ProviderUkPrnCheckActionFilter : IActionFilter
    {
        const string ProviderIdKey = "providerId";

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(ProviderUkPrnCheckAttribute), true)
                && !filterContext.ActionDescriptor.IsDefined(typeof(ProviderUkPrnCheckAttribute), true))
            {
                return;
            }

            if (filterContext.ActionParameters.ContainsKey(ProviderIdKey))
            {
                var providerIdFromAction = filterContext.ActionParameters[ProviderIdKey];
                if ($"{providerIdFromAction}" == string.Empty)
                {
                    throw new HttpException((int) HttpStatusCode.BadRequest, "Missing provider id");
                }

                var claimUkprn = filterContext.HttpContext.GetClaimValue("http://schemas.portal.com/ukprn");

                if ($"{providerIdFromAction}" != claimUkprn)
                {
                    throw new HttpException((int) HttpStatusCode.Forbidden,
                        $"Mismatched UKPRNs ({providerIdFromAction} requested on URL, user's claim contains {claimUkprn}");
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}

