using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MediatR;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProvider;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("~/signin", Name = "signin")]
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/" },
                    WsFederationAuthenticationDefaults.AuthenticationType);
            }
        }

        [Route("~/signout", Name = "signout")]
        public void SignOut()
        {
            var callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            var auth = Request.GetOwinContext().Authentication;

            auth.SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
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
        public async Task<ActionResult> Index()
        {
            //todo: move to orchestrator?
            var providerId = int.Parse(User.Identity.GetClaim("http://schemas.portal.com/ukprn"));

            var providers = await _mediator.SendAsync(new GetProviderQueryRequest { UKPRN = providerId });

            var provider = providers.ProvidersView.Providers.First();

            var model = new AccountHomeViewModel
            {
                ProviderId = provider.Ukprn,
                ProviderName = provider.ProviderName
            };

            return View(model);
        }
    }
}
