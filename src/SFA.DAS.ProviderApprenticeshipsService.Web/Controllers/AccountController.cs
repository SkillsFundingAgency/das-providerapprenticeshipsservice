using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Settings;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;

[Authorize]
public class AccountController : BaseController
{
    private readonly IAccountOrchestrator _accountOrchestrator;

    public AccountController(IAccountOrchestrator accountOrchestrator,
        ICookieStorageService<FlashMessageViewModel> flashMessage)
        : base(flashMessage)
    {
        _accountOrchestrator = accountOrchestrator;
    }

    [Route("~/signout", Name = RouteNames.SignOut)]
    public async Task ProviderSignOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(WsFederationDefaults.AuthenticationScheme, new AuthenticationProperties
        {
            RedirectUri = ""
        });
    }

    [HttpGet]
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
    [Route("~/notification-settings", Name = RouteNames.GetNotificationSettings)]
    public async Task<IActionResult> NotificationSettings()
    {
        var userRef = User.Identity.GetClaim(DasClaimTypes.Upn);
        var providerId = int.Parse(User.Identity.GetClaim(DasClaimTypes.Ukprn));

        var model = await _accountOrchestrator.GetNotificationSettings(userRef);
        model.ProviderId = providerId;

        var flashMessage = GetFlashMessageViewModelFromCookie();
        if (flashMessage != null)
        {
            model.FlashMessage = flashMessage;
        }

        return View(model);
    }

    [HttpPost]
    [Route("~/notification-settings", Name = RouteNames.PostNotificationSettings)]
    public async Task<IActionResult> NotificationSettings(NotificationSettingsViewModel model)
    {
        await _accountOrchestrator.UpdateNotificationSettings(model);
        SetInfoMessage("Settings updated", FlashMessageSeverityLevel.Info);

        return RedirectToRoute(RouteNames.GetNotificationSettings);
    }

    [HttpGet]
    [Route("~/notifications/unsubscribe", Name = RouteNames.UnsubscribeNotifications)]
    public async Task<IActionResult> NotificationUnsubscribe()
    {
        var userRef = User.Identity.GetClaim(DasClaimTypes.Upn);

        var url = Url.Action("NotificationSettings");
        var model = await _accountOrchestrator.Unsubscribe(userRef, url);

        return View(model);
    }

    [HttpGet]
    [Route("~/change-signin-details")]
    public ActionResult ChangeSignInDetails()
    {
        return View();
    }
}