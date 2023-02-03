using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Services;
using System;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes
{
    public class RoatpCourseManagementCheckActionFilter : IActionFilter
    {
        private readonly IGetRoatpBetaProviderService _roatpProviderService;
        private readonly HttpContext _httpContext;

        public RoatpCourseManagementCheckActionFilter(IGetRoatpBetaProviderService roatpProviderService, HttpContext httpContext)
        {
            _roatpProviderService = roatpProviderService;
            _httpContext = httpContext;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ukprn = _httpContext.User.Identity.GetClaim(DasClaimTypes.Ukprn);

             if (string.IsNullOrEmpty(ukprn) || !int.TryParse(ukprn, out var ukprnValue ))
            {
                throw new Exception("Missing provider id");
            }

             var isRoatpCourseManagementLinkEnabled = _roatpProviderService.IsUkprnEnabled(ukprnValue);
             if (!_httpContext.Items.ContainsKey(RoatpConstants.IsCourseManagementLinkEnabled))
                _httpContext.Items.Add(RoatpConstants.IsCourseManagementLinkEnabled, isRoatpCourseManagementLinkEnabled);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // No code required for OnActionExecuted, but method required by IActionFilter
            // SonarCloud wanted a comment
        }
    }
}