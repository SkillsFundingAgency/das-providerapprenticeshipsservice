using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;

[Route("{controller}")]
public class HomeController : Controller
{

    [HttpGet]
    [Route("help")]
    public IActionResult Help()
    {
        private readonly ProviderApprenticeshipsServiceConfiguration _providerApprenticeshipsServiceConfiguration;

        public HomeController(ProviderApprenticeshipsServiceConfiguration providerApprenticeshipsServiceConfiguration)
        {
            _providerApprenticeshipsServiceConfiguration = providerApprenticeshipsServiceConfiguration;
        }

    [HttpGet]
    [Route("/", Name = RouteNames.Home)]
    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated) return RedirectToRoute(RouteNames.AccountHome);

        [HttpGet]
        [Route("/", Name = RouteNames.Home)]
        public IActionResult Index()
        {
            return View(new HomeViewModel { UseDfESignIn = _providerApprenticeshipsServiceConfiguration.UseDfESignIn });
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
}