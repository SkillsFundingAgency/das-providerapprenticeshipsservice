using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using System.Web.Mvc;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [AllowAllRoles]
    public class HomeController : Controller
    {
        [Route("~/help", Name = "help")]
        public ActionResult Help()
        {
            return View();
        }

        [Route("~/", Name = "home")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("~/terms", Name = "terms")]
        public ActionResult Terms()
        {
            return View();
        }

        [Route("~/privacy", Name = "privacy")]
        public ActionResult Privacy()
        {
            return View();
        }

        [Route("~/cookies", Name = "cookies")]
        public ActionResult Cookies()
        {
            return View();
        }

        [Route("~/cookie-details", Name = "cookie-details")]
        public ActionResult CookieDetails()
        {
            return View();
        }
    }
}
