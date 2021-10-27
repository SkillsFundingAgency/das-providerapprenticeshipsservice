using System.Web.Mvc;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [AllowAllRoles]
    public class HomeController : BaseController
    {
        private readonly ILog _logger;

        public HomeController(ICookieStorageService<FlashMessageViewModel> flashMessage, ILog logger) : base(flashMessage)
        {
            _logger = logger;
        }

        [Route("~/help", Name = "help")]
        public ActionResult Help()
        {
            return View();
        }

        [Route("~/", Name = "home")]
        public ActionResult Index()
        {
            _logger.Info($"Provider Apprenticeship Home Index: Authenticated {User.Identity.IsAuthenticated}");
            
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
