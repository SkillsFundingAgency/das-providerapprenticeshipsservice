using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes
{
    public class DasRoleCheckActionFilter : IActionFilter
    {
        private string[] ValidDasRoles = new string[] { "DAA", "DAB", "DAC", "DAV" };

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(DasRoleCheckAttribute), true)
                && !filterContext.ActionDescriptor.IsDefined(typeof(DasRoleCheckAttribute), true))
            {
                return;
            }

            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(DasRoleCheckExemptAttribute), true)
                || filterContext.ActionDescriptor.IsDefined(typeof(DasRoleCheckExemptAttribute), true))
            {
                return;
            }

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, $"User not authenticated when checking for valid Das role");
            }

            if (!filterContext.HttpContext.HasAnyClaimValue("http://schemas.portal.com/service", ValidDasRoles))
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, $"Service claim must be one of [{string.Join(",", ValidDasRoles)}] for valid Das role");
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}