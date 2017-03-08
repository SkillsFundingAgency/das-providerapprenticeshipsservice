using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes
{
    public class DasRoleCheckActionFilter : IActionFilter
    {
        private const string RequiredUserRole = "DAA";

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(DasRoleCheckAttribute), true)
                && !filterContext.ActionDescriptor.IsDefined(typeof(DasRoleCheckAttribute), true))
            {
                return;
            }

            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(DasRoleCheckExemptAttribute), true)
                ||filterContext.ActionDescriptor.IsDefined(typeof(DasRoleCheckExemptAttribute), true))
            {
                return;
            }

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, $"User not authenticated on check for {RequiredUserRole} claim");
            }

            if (!filterContext.HttpContext.HasClaimValue("http://schemas.portal.com/service", RequiredUserRole))
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, $"Missing {RequiredUserRole} claim");
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}