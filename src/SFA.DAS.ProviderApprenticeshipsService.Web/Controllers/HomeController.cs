using System;
using System.Web.Mvc;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public class HomeController : BaseController
    {
        [Route("~/", Name = "home")]
        public ActionResult Index()
        {
            // HACK: There is currently an issue with login and the session provider.
            // This is a work-around to enable login to function.
            Session["TODO"] = "";

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
