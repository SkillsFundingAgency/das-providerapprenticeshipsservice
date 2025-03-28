using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Account;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Settings;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;

[Authorize]
public class AccountController(
    IAccountOrchestrator accountOrchestrator,
    ICookieStorageService<FlashMessageViewModel> flashMessage,
    IConfiguration configuration,
    ILogger<AccountController> logger)
    : BaseController(flashMessage)
{
    [Route("~/signout", Name = RouteNames.SignOut)]
    public async Task ProviderSignOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties());
    }

    [HttpGet]
    [Route("~/account", Name = RouteNames.AccountHome)]
    public async Task<IActionResult> Index(string message)
    {
        var providerId = int.Parse(User.Identity.GetClaim(DasClaimTypes.Ukprn));

        var model = await accountOrchestrator.GetAccountHomeViewModel(providerId);

        if (!string.IsNullOrEmpty(message))
        {
            model.Message = WebUtility.UrlDecode(message);
        }

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
        var userRef = User.GetUserRef();
        var email = User.Identity.GetClaim(DasClaimTypes.DfEEmail);
        var providerId = int.Parse(User.Identity.GetClaim(DasClaimTypes.Ukprn));
        
        logger.LogInformation("AccountController.NotificationSettings. userRef: '{UserRef}'. email: '{Email}'. claims: '{Claims}'.", 
            userRef, 
            email, 
            JsonConvert.SerializeObject(User.Claims.Select(x => new
            {
                x.Type,
                x.Value
            })));

        var model = await accountOrchestrator.GetNotificationSettings(userRef, email);
        model.ProviderId = providerId;

        var flashMessage = GetFlashMessageViewModelFromCookie();
        if (flashMessage != null)
        {
            model.FlashMessage = flashMessage;
        }

        var name = User.Identity.GetClaim(DasClaimTypes.Upn) ?? User.Identity.GetClaim(DasClaimTypes.DisplayName);
        foreach (var userNotificationSetting in model.NotificationSettings)
        {
            userNotificationSetting.Name = name;
        }

        return View(model);
    }

    [HttpPost]
    [Route("~/notification-settings", Name = RouteNames.PostNotificationSettings)]
    public async Task<IActionResult> NotificationSettings(NotificationSettingsViewModel model)
    {
        await accountOrchestrator.UpdateNotificationSettings(model);
        SetInfoMessage("Settings updated", FlashMessageSeverityLevel.Info);

        return RedirectToRoute(RouteNames.GetNotificationSettings);
    }

    [HttpGet]
    [Route("~/notifications/unsubscribe", Name = RouteNames.UnsubscribeNotifications)]
    public async Task<IActionResult> NotificationUnsubscribe()
    {
        var userRef = User.GetUserRef();

        var url = Url.Action("NotificationSettings");
        var model = await accountOrchestrator.Unsubscribe(userRef, url);

        return View(model);
    }

    [HttpGet]
    [Route("~/change-signin-details")]
    public ActionResult ChangeSignInDetails()
    {
        return View(new ChangeOfDetailsViewModel(configuration["ResourceEnvironmentName"]));
    }
}