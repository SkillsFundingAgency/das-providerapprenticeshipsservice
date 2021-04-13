using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes
{
    public class AllowAllRolesAttribute : ActionFilterAttribute
    {

    }

    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            filterContext.HttpContext.Items["ActionDescriptor"] = filterContext.ActionDescriptor;
            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Items["ActionDescriptor"] is ActionDescriptor actionDescriptor)
            {
                if ((actionDescriptor.GetCustomAttributes(typeof(AllowAllRolesAttribute), true).Any() ||
                    actionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AllowAllRolesAttribute), true).Any()) &&
                    httpContext.User.Identity.HasServiceClaim())
                {
                    return true;
                }
            }

            return base.AuthorizeCore(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, $"Access denied for user {filterContext.HttpContext.GetClaimValue(DasClaimTypes.Name)}");
            }

            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}