using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Application.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [RoleAuthorize(Roles = nameof(RoleNames.DasPermission))]
    public abstract class BaseController : Controller
    {
        private const string FlashMessageCookieName = "sfa-das-employerapprenticeshipsservice-flashmessage";
        private readonly ICookieStorageService<FlashMessageViewModel> _flashMessage;
        private readonly ProviderApprenticeshipsServiceConfiguration _configuration;

        protected BaseController(ICookieStorageService<FlashMessageViewModel> flashMessage, ProviderApprenticeshipsServiceConfiguration configuration)
        {
            _flashMessage = flashMessage;
            _configuration = configuration;
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
                if (_configuration != null && _configuration.UseDfESignIn)
                {
                    ViewBag.UseDfESignIn = true;
                }
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
