using System;
using System.Web.Mvc;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public class HomeController : Controller
    {
        [Route("~/", Name = "home")]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated) return RedirectToRoute("account-home");

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
    }
}
