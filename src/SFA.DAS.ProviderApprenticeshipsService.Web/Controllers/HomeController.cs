using System.Web.Mvc;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [DasRoleCheckExempt]
    public class HomeController : BaseController
    {
        public HomeController(ICookieStorageService<FlashMessageViewModel> flashMessage) : base(flashMessage)
        {
        }

        [Route("~/help", Name = "help")]
        public ActionResult Help()
        {
            return View();
        }

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
