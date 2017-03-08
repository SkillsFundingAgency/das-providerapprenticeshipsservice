using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [DasRoleCheckExempt]
    public class HomeController : BaseController
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
