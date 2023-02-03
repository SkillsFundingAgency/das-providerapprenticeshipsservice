using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.ProviderApprenticeshipsService.Application.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Authorize(Roles = nameof(RoleNames.DasPermission))]
    public abstract class BaseController : Controller
    {
        private const string FlashMessageCookieName = "sfa-das-providerapprenticeshipsservice-flashmessage";
        private readonly ICookieStorageService<FlashMessageViewModel> _flashMessage;

        protected BaseController(ICookieStorageService<FlashMessageViewModel> flashMessage)
        {
            _flashMessage = flashMessage;
        }

        protected string CurrentUserId;

        protected void SetInfoMessage(string messageText, FlashMessageSeverityLevel level)
        {
            var message = new FlashMessageViewModel
            {
                Message = messageText,
                Severity = level
            };
            _flashMessage.Delete(FlashMessageCookieName);

            _flashMessage.Create(message, FlashMessageCookieName);
        }

        public FlashMessageViewModel GetFlashMessageViewModelFromCookie()
        {
            var flashMessageViewModelFromCookie = _flashMessage.Get(FlashMessageCookieName);
            _flashMessage.Delete(FlashMessageCookieName);
            return flashMessageViewModelFromCookie;
        }

        /*
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
                CurrentUserId = filterContext.HttpContext.GetClaimValue(DasClaimTypes.Upn);
            }
        }

        protected SignInUserModel GetSignedInUser()
        {
            return new SignInUserModel
            {
                DisplayName = Request.GetClaimValue(DasClaimTypes.DisplayName),
                Email = HttpContext.GetClaimValue(DasClaimTypes.Email)
            };
        }
        */
    }
}
