using System.Net;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Attributes
{
    public class RoatpCourseManagementCheckActionFilter : IActionFilter
    {
        private readonly IGetRoatpBetaProviderService _roatpProviderService;
        
        public RoatpCourseManagementCheckActionFilter(IGetRoatpBetaProviderService roatpProviderService)
        {
            _roatpProviderService = roatpProviderService;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // skip the global filter RoatpCourseManagementCheckActionFilter attribute for the controllers and action methods.
            if (!filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(RoatpCourseManagementCheckActionFilter), true)
                && !filterContext.ActionDescriptor.IsDefined(typeof(RoatpCourseManagementCheckActionFilter), true))
            {
                return;
            }

            var ukprn = filterContext.HttpContext.GetClaimValue("http://schemas.portal.com/ukprn");
            
             if (string.IsNullOrEmpty(ukprn) || !int.TryParse(ukprn, out var ukprnValue ))
            {
                throw new HttpException((int)HttpStatusCode.BadRequest, "Missing provider id");
            }

             var isRoatpCourseManagementLinkEnabled = _roatpProviderService.IsUkprnEnabled(ukprnValue);
             if (!HttpContext.Current.Items.Contains(RoatpConstants.IsCourseManagementLinkEnabled))
                    HttpContext.Current.Items.Add(RoatpConstants.IsCourseManagementLinkEnabled, isRoatpCourseManagementLinkEnabled);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // No code required for OnActionExecuted, but method required by IActionFilter
            // SonarCloud wanted a comment
        }
    }
}