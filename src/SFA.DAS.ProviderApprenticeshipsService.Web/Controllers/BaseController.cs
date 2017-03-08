using System.Security.Claims;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [DasRoleCheck]
    public abstract class BaseController : Controller
    {
        protected string CurrentUserId = null;

        protected void SetInfoMessage(string messageText, FlashMessageSeverityLevel level)
        {
            var message = new FlashMessageViewModel
            {
                Message = messageText,
                Severity = level
            };

            var flashMessage = JsonConvert.SerializeObject(message);
            TempData["InfoMessage"] = flashMessage;
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is InvalidStateException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = RedirectToAction("InvalidState", "Error");
            }
        }
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                CurrentUserId = filterContext.HttpContext.GetClaimValue(ClaimTypes.Upn);
            }
        }
    }
}
