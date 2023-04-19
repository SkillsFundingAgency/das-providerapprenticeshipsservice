using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Settings;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;

public class AccountController : BaseController
{
    private readonly IAccountOrchestrator _accountOrchestrator;
    private readonly LinkGenerator _linkGenerator;

    public AccountController(IAccountOrchestrator accountOrchestrator,
        LinkGenerator linkGenerator,
        ICookieStorageService<FlashMessageViewModel> flashMessage) 
        : base(flashMessage)
    {
        _accountOrchestrator = accountOrchestrator;
        _linkGenerator = linkGenerator;
    }

    [AllowAllRoles]
    [Route("~/signin", Name = RouteNames.SignIn)]
    public IActionResult SignIn()
    {
        if (!User.Identity.IsAuthenticated)
        {
            HttpContext.ChallengeAsync(WsFederationDefaults.AuthenticationScheme);
        }
        return RedirectToRoute(RouteNames.Home);
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
        SignOut(authenticationProperties, 
            CookieAuthenticationDefaults.AuthenticationScheme,
            WsFederationDefaults.AuthenticationScheme);

        if (User.Identity.IsAuthenticated)
        {
            return RedirectToRoute(RouteNames.AccountHome);
        }

        return RedirectToRoute(RouteNames.Home);
    }

    [HttpGet]
    [Authorize]
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

    [HttpGet]
    [Authorize]
    [Route("~/change-signin-details")]
    public ActionResult ChangeSignInDetails()
    {
        return View();
    }
}