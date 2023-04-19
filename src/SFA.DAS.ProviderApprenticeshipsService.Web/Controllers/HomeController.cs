using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers;

[Route("{controller}")]
public class HomeController : Controller
{

    [HttpGet]
    [Route("help")]
    public IActionResult Help()
    {
        return View("Help");
    }

    [HttpGet]
    [Route("/", Name = RouteNames.Home)]
    public IActionResult Index()
    {
        if (User.Identity.IsAuthenticated) return RedirectToRoute(RouteNames.AccountHome);

        return View();
    }

    [Route("terms", Name = "terms")]
    public IActionResult Terms()
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
}