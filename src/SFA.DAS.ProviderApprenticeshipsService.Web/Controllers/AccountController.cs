using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Settings;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly AccountOrchestrator _accountOrchestrator;

        public AccountController(AccountOrchestrator accountOrchestrator, ICookieStorageService<FlashMessageViewModel> flashMessage) : base(flashMessage)
        {
            _accountOrchestrator = accountOrchestrator;
        }

        [AllowAllRoles]
        [Route("~/signin", Name = "signin")]
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties {RedirectUri = "/"},
                    WsFederationAuthenticationDefaults.AuthenticationType);
            }
        }

        [AllowAllRoles]
        [Route("~/signout", Name = "signout")]
        public void SignOut()
        {
            var callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            var auth = Request.GetOwinContext().Authentication;

            auth.SignOut(
                new AuthenticationProperties {RedirectUri = callbackUrl},
                WsFederationAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

        [AllowAllRoles]
        public ActionResult SignOutCallback()
        {
            if (Request.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToRoute("account-home");
            }

            return RedirectToRoute("home");
        }

        [Authorize]
        [Route("~/account", Name = "account-home")]
        public async Task<ActionResult> Index(string message)
        {
            var providerId = int.Parse(User.Identity.GetClaim("http://schemas.portal.com/ukprn"));

            var model = await _accountOrchestrator.GetAccountHomeViewModel(providerId);
                       
            if (!string.IsNullOrEmpty(message))
                model.Message = HttpUtility.UrlDecode(message);

            switch (model.AccountStatus)
            {
                case AccountStatus.NotListed:
                    return View("NoAgreement", model);

                case AccountStatus.NoAgreement:
                    return View("NoAccount");

                case AccountStatus.Active:
                default:
                    return View(model);
            }
        }

        [Authorize]
        [Route("~/notification-settings")]
        public async Task<ActionResult> NotificationSettings()
        {
            var u = User.Identity.GetClaim(DasClaimTypes.Upn);
            var providerId = int.Parse(User.Identity.GetClaim(DasClaimTypes.Ukprn));

            var model = await _accountOrchestrator.GetNotificationSettings(u);
            model.ProviderId = providerId;
            
            var flashMesssage = GetFlashMessageViewModelFromCookie();
            if (flashMesssage != null)
            {
                model.FlashMessage = flashMesssage;
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [Route("~/notification-settings")]
        public async Task<ActionResult> NotificationSettings(NotificationSettingsViewModel model)
        {
            await _accountOrchestrator.UpdateNotificationSettings(model);
            SetInfoMessage("Settings updated", FlashMessageSeverityLevel.Info);
            return RedirectToAction("NotificationSettings");
        }

        [HttpGet]
        [Authorize]
        [Route("~/notifications/unsubscribe")]
        public async Task<ActionResult> NotificationUnsubscribe()
        {
            var userRef = User.Identity.GetClaim(DasClaimTypes.Upn);

            var url = Url.Action("NotificationSettings");
            var model = await _accountOrchestrator.Unsubscribe(userRef, url);

            return View(model);
        }

        [HttpGet]
        [Authorize]
        [Route("~/change-signin-details")]
        public ActionResult ChangeSignInDetails()
        {
            return View();
        }
    }
}
