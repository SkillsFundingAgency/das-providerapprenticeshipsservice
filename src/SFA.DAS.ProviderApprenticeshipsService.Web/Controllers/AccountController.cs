using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Settings;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using static IdentityServer3.Core.Constants;
using Microsoft.AspNetCore.Routing;
using NLog;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly AccountOrchestrator _accountOrchestrator;
        private readonly LinkGenerator _linkGenerator;
        private readonly IAuthenticationServiceWrapper _authenticationService;

        public AccountController(AccountOrchestrator accountOrchestrator,
            LinkGenerator linkGenerator,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            IAuthenticationServiceWrapper authenticationService) 
            : base(flashMessage)
        {
            _accountOrchestrator = accountOrchestrator;
            _linkGenerator = linkGenerator;
            _authenticationService = authenticationService;
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
        public async Task<IActionResult> SignOut()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");

            var callbackUrl = _linkGenerator.GetPathByAction("Index", "Account", values: new
            {
                message = ""
            });
            var authenticationProperties = new AuthenticationProperties { RedirectUri = callbackUrl };
            authenticationProperties.Parameters.Clear();
            authenticationProperties.Parameters.Add("id_token", idToken);
            return SignOut(
                authenticationProperties, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
            /*
            var auth = _httpContext.Request.Query.GetOwinContext().Authentication;

            auth.SignOut(
                new AuthenticationProperties {RedirectUri = callbackUrl},
                WsFederationAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);

            if (Request.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToRoute("account-home");
            }

            return RedirectToRoute("home");
            */
        }

        [HttpGet]
        [Authorize]
        [Route("~/account", Name = "account-home")]
        public async Task<IActionResult> Index(string message)
        {
            var providerId = int.Parse(User.Identity.GetClaim("http://schemas.portal.com/ukprn"));

            var model = await _accountOrchestrator.GetAccountHomeViewModel(providerId);
                       
            if (!string.IsNullOrEmpty(message))
                model.Message = WebUtility.UrlDecode(message);

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

        [HttpGet]
        [Authorize]
        [Route("~/notification-settings")]
        public async Task<IActionResult> NotificationSettings()
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
        public async Task<IActionResult> NotificationSettings(NotificationSettingsViewModel model)
        {
            await _accountOrchestrator.UpdateNotificationSettings(model);
            SetInfoMessage("Settings updated", FlashMessageSeverityLevel.Info);
            return RedirectToAction("NotificationSettings");
        }

        [HttpGet]
        [Authorize]
        [Route("~/notifications/unsubscribe")]
        public async Task<IActionResult> NotificationUnsubscribe()
        {
            var userRef = User.Identity.GetClaim(DasClaimTypes.Upn);

            var url = Url.Action("NotificationSettings");
            var model = await _accountOrchestrator.Unsubscribe(userRef, url);

            return View(model);
        }
    }
}
