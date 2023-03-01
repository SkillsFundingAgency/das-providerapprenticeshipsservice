﻿using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using System.Web.Mvc;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [AllowAllRoles]
    public class HomeController : BaseController
    {
        public HomeController(ICookieStorageService<FlashMessageViewModel> flashMessage, ProviderApprenticeshipsServiceConfiguration configuration) : base(flashMessage, configuration)
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
