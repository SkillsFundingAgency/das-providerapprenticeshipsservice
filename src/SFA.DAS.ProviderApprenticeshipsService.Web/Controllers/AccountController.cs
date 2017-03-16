﻿using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly AccountOrchestrator _accountOrchestrator;

        public AccountController(AccountOrchestrator accountOrchestrator)
        {
            _accountOrchestrator = accountOrchestrator;
        }

        [DasRoleCheckExempt]
        [Route("~/signin", Name = "signin")]
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties {RedirectUri = "/"},
                    WsFederationAuthenticationDefaults.AuthenticationType);
            }
        }

        [DasRoleCheckExempt]
        [Route("~/signout", Name = "signout")]
        public void SignOut()
        {
            var callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            var auth = Request.GetOwinContext().Authentication;

            auth.SignOut(
                new AuthenticationProperties {RedirectUri = callbackUrl},
                WsFederationAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

        [DasRoleCheckExempt]
        public ActionResult SignOutCallback()
        {
            if (Request.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToRoute("account-home");
            }

            return RedirectToRoute("home");
        }

        [Authorize]
        [Route("~/account", Name = "account-home")]
        public async Task<ActionResult> Index(string message)
        {
            var providerId = int.Parse(User.Identity.GetClaim("http://schemas.portal.com/ukprn"));

            var model = await _accountOrchestrator.GetProvider(providerId);
            if(!string.IsNullOrEmpty(message))
                model.Message = HttpUtility.UrlDecode(message);

            switch (model.AccountStatus)
            {
                case AccountStatus.NotListed:
                    return View("NoAgreement", model);

                case AccountStatus.NoAgreement:
                    return View("NoAccount");

                case AccountStatus.Active:
                default:
                    return View(model);
            }
        }
    }
}
