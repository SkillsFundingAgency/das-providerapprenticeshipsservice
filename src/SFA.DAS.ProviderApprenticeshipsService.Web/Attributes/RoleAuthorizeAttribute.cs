using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.Authorization.Context;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes
{
    public class AllowAllRolesAttribute : ActionFilterAttribute
    {

    }

    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        /* TODO - MAC-201
        public void OnAuthorization(HttpContext httpContext)
        {
            var actionDescriptor = httpContext.Items["ActionDescriptor"] as ActionDescriptor;
            //base.OnAuthorization(actionDescriptor);
        }

        protected bool AuthorizeCore(HttpContext httpContext)
        {
            if (httpContext.Items["ActionDescriptor"] is ActionDescriptor actionDescriptor)
            {
                if (actionDescriptor.EndpointMetadata.OfType<AllowAllRolesAttribute>().Any()
                    // ||
                    // actionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AllowAllRolesAttribute), true).Any())
                    &&
                    httpContext.User.Identity.HasServiceClaim())
                {
                    return true;
                }
            }

            return base.AuthorizeCore(httpContext);
        }

        protected void HandleUnauthorizedRequest(HttpContext httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                throw new HttpException((int)HttpStatusCode.Forbidden, $"Access denied for user {filterContext.HttpContext.GetClaimValue(DasClaimTypes.Name)}");
            }

            base.HandleUnauthorizedRequest(filterContext);
        }
        */
    }
}