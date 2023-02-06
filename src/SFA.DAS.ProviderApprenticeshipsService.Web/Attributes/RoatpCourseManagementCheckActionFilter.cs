using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Services;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes
{
    public class RoatpCourseManagementCheckActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var roatpProviderService = filterContext.HttpContext.RequestServices.GetService<IGetRoatpBetaProviderService>();
            var ukprn = filterContext.HttpContext.User.Identity.GetClaim(DasClaimTypes.Ukprn);

             if (string.IsNullOrEmpty(ukprn) || !int.TryParse(ukprn, out var ukprnValue ))
            {
                throw new Exception("Missing provider id");
            }

             var isRoatpCourseManagementLinkEnabled = roatpProviderService.IsUkprnEnabled(ukprnValue);
             if (!filterContext.HttpContext.Items.ContainsKey(RoatpConstants.IsCourseManagementLinkEnabled))
                filterContext.HttpContext.Items.Add(RoatpConstants.IsCourseManagementLinkEnabled, isRoatpCourseManagementLinkEnabled);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // No code required for OnActionExecuted, but method required by IActionFilter
            // SonarCloud wanted a comment
        }
    }
}