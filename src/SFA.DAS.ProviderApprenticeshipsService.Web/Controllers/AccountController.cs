using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Settings;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;
using Microsoft.AspNetCore.Authentication.WsFederation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAccountOrchestrator _accountOrchestrator;
        private readonly LinkGenerator _linkGenerator;
        private readonly IAuthenticationServiceWrapper _authenticationService;
        private readonly ProviderApprenticeshipsServiceConfiguration _providerApprenticeshipsServiceConfiguration;

        public AccountController(IAccountOrchestrator accountOrchestrator,
            LinkGenerator linkGenerator,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            IAuthenticationServiceWrapper authenticationService,
            ProviderApprenticeshipsServiceConfiguration providerApprenticeshipsServiceConfiguration) 
            : base(flashMessage)
        {
            _accountOrchestrator = accountOrchestrator;
            _linkGenerator = linkGenerator;
            _authenticationService = authenticationService;
            _providerApprenticeshipsServiceConfiguration = providerApprenticeshipsServiceConfiguration;
        }

        [AllowAllRoles]
        [Route("~/signout", Name = RouteNames.SignOut)]
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

            var authScheme = _providerApprenticeshipsServiceConfiguration.UseDfESignIn
                ? OpenIdConnectDefaults.AuthenticationScheme
                : WsFederationDefaults.AuthenticationScheme;

            SignOut(authenticationProperties, 
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    authScheme);

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoute(RouteNames.AccountHome);
            }

            return RedirectToRoute(RouteNames.Home);
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
        [ServiceFilter(typeof(RoatpCourseManagementCheckActionFilter))]
        [Route("~/account", Name = RouteNames.AccountHome)]
        public async Task<IActionResult> Index(string message)
        {
            var providerId = int.Parse(User.Identity.GetClaim(DasClaimTypes.Ukprn));

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
        [Route("~/notification-settings", Name = RouteNames.GetNotificationSettings)]
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
        [Route("~/notification-settings", Name = RouteNames.PostNotificationSettings)]
        public async Task<IActionResult> NotificationSettings(NotificationSettingsViewModel model)
        {
            await _accountOrchestrator.UpdateNotificationSettings(model);
            SetInfoMessage("Settings updated", FlashMessageSeverityLevel.Info);
            return RedirectToRoute(RouteNames.GetNotificationSettings);
        }

        [HttpGet]
        [Authorize]
        [Route("~/notifications/unsubscribe", Name = RouteNames.UnsubscribeNotifications)]
        public async Task<IActionResult> NotificationUnsubscribe()
        {
            var userRef = User.Identity.GetClaim(DasClaimTypes.Upn);

            var url = Url.Action("NotificationSettings");
            var model = await _accountOrchestrator.Unsubscribe(userRef, url);

            return View(model);
        }
    }
}
