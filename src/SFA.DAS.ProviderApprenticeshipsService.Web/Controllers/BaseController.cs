using System.Security.Claims;
using System.Web.Mvc;
using Newtonsoft.Json;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [DasRoleCheck]
    public abstract class BaseController : Controller
    {
        private const string FlashMessageCookieName = "sfa-das-employerapprenticeshipsservice-flashmessage";
        private readonly ICookieStorageService<FlashMessageViewModel> _flashMessage;

        protected BaseController(ICookieStorageService<FlashMessageViewModel> flashMessage)
        {
            _flashMessage = flashMessage;
        }

        protected string CurrentUserId = null;

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
                DisplayName = HttpContext.GetClaimValue(DasClaimTypes.DisplayName),
                Email = HttpContext.GetClaimValue(DasClaimTypes.Email)
            };
        }
    }
}
