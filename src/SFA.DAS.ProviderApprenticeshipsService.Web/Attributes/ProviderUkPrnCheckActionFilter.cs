using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes
{
    public class ProviderUkPrnCheckActionFilter : IActionFilter
    {
        const string ProviderIdKey = "providerId";

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(ProviderUkPrnCheckAttribute), true).Any())
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
                        $"User ukprn: {providerIdFromAction}, Resources ukprn: {claimUkprn}");
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}

