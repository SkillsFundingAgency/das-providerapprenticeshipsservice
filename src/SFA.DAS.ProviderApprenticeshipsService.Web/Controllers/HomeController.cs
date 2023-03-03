using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Route("{controller}")]
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

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
}
