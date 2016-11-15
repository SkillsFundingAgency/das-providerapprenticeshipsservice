using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public class AccountController : Controller
    {
        [Route("~/signin", Name = "signin")]
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties {RedirectUri = "/"},
                    WsFederationAuthenticationDefaults.AuthenticationType);
            }
        }

        [Route("~/signout", Name = "signout")]
        public void SignOut()
        {
            var callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            var auth = Request.GetOwinContext().Authentication;

            auth.SignOut(
                new AuthenticationProperties {RedirectUri = callbackUrl},
                WsFederationAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

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
        public ActionResult Index()
        {
            //todo: account home model needs to read more claims
            var model = new AccountHomeViewModel
            {
                ProviderId = User.Identity.GetClaim("http://schemas.portal.com/ukprn"),
                ProviderName = "Hackney skills and training"
            };

            return View(model);
        }
    }
}
