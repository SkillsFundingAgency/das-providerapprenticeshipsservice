using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.DfESignIn.Auth.Constants;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;

[Route("")]
public class HomeController : Controller
{
    private readonly IAuthenticationOrchestrator _authenticationOrchestrator;
    private readonly ProviderApprenticeshipsServiceConfiguration _providerApprenticeshipsServiceConfiguration;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ProviderApprenticeshipsServiceConfiguration providerApprenticeshipsServiceConfiguration,
        IAuthenticationOrchestrator authenticationOrchestrator, ILogger<HomeController> logger)
    {
        _providerApprenticeshipsServiceConfiguration = providerApprenticeshipsServiceConfiguration;
        _authenticationOrchestrator = authenticationOrchestrator;
        _logger = logger;
    }

    [Route("~/signed-out", Name = "signed-out")]
    [AllowAnonymous]
    public IActionResult ProviderSignedOut()
    {
        var autoSignOut = TempData["AutoSignOut"] as bool? ?? false;
        return autoSignOut ? View("AutoSignOut") : RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("help")]
    public IActionResult Help()
    {
        return View();
    }

    [HttpGet]
    [Route("/", Name = RouteNames.Home)]
    public IActionResult Index()
    {
        var useDfESignIn = _providerApprenticeshipsServiceConfiguration.UseDfESignIn;
        var isAuthenticated = User.Identity?.IsAuthenticated;
        var referer = Request.Headers["Referer"].ToString();

        _logger.LogInformation(
            "Home Index: UseDfESignIn={UseDfESignIn}, IsAuthenticated={IsAuthenticated}, Referer={Referer}",
            useDfESignIn,
            isAuthenticated,
            referer);

        // if the DfESignIn is enabled or user is authenticated, then redirect the user to the Start Page.
        if (!_providerApprenticeshipsServiceConfiguration.UseDfESignIn || User.Identity is { IsAuthenticated: true })
            return RedirectToRoute(RouteNames.AccountHome);

        return View();
    }

    [Route("terms", Name = "terms")]
    public ActionResult Terms()
    {
        return View();
    }

    [Route("accessibility", Name = RouteNames.Accessibility)]
    public ActionResult Accessibility()
    {
        return View();
    }

    [Route("privacy", Name = "privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [Route("cookies", Name = "cookies")]
    public IActionResult Cookies()
    {
        return View();
    }


    [Route("cookie-details", Name = "cookie-details")]
    public IActionResult CookieDetails()
    {
        return View();
    }

    [HttpGet]
    [Authorize]
    [Route("~/signin", Name = RouteNames.SignIn)]
    public async Task<IActionResult> SignIn()
    {
        var useDfESignIn = _providerApprenticeshipsServiceConfiguration.UseDfESignIn;
        if (User.Identity is { IsAuthenticated: false })
        {
            await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
        else if (useDfESignIn)
        {
            // maps the roles and save the claim details in the repository.
            await SaveIdentityAttributes();
        }

        return RedirectToRoute(RouteNames.AccountHome);
    }

    /// <summary>
    ///     Method to iterate the claims roles and save in the repository.
    /// </summary>
    /// <returns>Task</returns>
    private async Task SaveIdentityAttributes()
    {
        var claimsPrincipal = User.Identities.FirstOrDefault();

        if (claimsPrincipal != null)
        {
            var id = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimName.Sub)?.Value;
            var displayName = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == DasClaimTypes.DisplayName)
                ?.Value;
            var ukPrn = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == DasClaimTypes.Ukprn)?.Value;
            var email = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimName.Email)?.Value;

            await _authenticationOrchestrator.SaveIdentityAttributes(id, ukPrn, displayName, email);
        }
    }
}