using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.DfESignIn.Auth.Constants;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Controllers
{
    [Route("{controller}")]
    public class HomeController : Controller
    {
        private readonly ProviderApprenticeshipsServiceConfiguration _providerApprenticeshipsServiceConfiguration;
        private readonly IAuthenticationOrchestrator _authenticationOrchestrator;

        public HomeController(ProviderApprenticeshipsServiceConfiguration providerApprenticeshipsServiceConfiguration, IAuthenticationOrchestrator authenticationOrchestrator)
        {
            _providerApprenticeshipsServiceConfiguration = providerApprenticeshipsServiceConfiguration;
            _authenticationOrchestrator = authenticationOrchestrator;
        }

        [HttpGet]
        [Route("help")]
        public IActionResult Help()
        {
            return View("Help");
        }

        [HttpGet]
        [Route("/", Name = RouteNames.Home)]
        public IActionResult Index()
        {
            return View(new HomeViewModel { UseDfESignIn = _providerApprenticeshipsServiceConfiguration.UseDfESignIn });
        }

        [Route("terms", Name = "terms")]
        public IActionResult Terms()
        {
            return View();
        }

        [Route("privacy", Name = "privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("cookies", Name = "cookies")]
        public IActionResult Cookies()
        {
            return View();
        }

        [Route("cookie-details", Name = "cookie-details")]
        public IActionResult CookieDetails()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        [AllowAllRoles]
        [Route("~/signin", Name = RouteNames.SignIn)]
        public async Task<IActionResult> SignIn()
        {
            var useDfESignIn = _providerApprenticeshipsServiceConfiguration.UseDfESignIn;
            if (User.Identity is { IsAuthenticated: false })
            {
                // choose the authScheme based on the SSO.
                var authScheme = useDfESignIn
                    ? OpenIdConnectDefaults.AuthenticationScheme
                    : WsFederationDefaults.AuthenticationScheme;

                await HttpContext.ChallengeAsync(authScheme);
            }
            else if(useDfESignIn)
            {
                // maps the roles and save the claim details in the repository.
                await SaveIdentityAttributes();
            }
            return RedirectToRoute(RouteNames.AccountHome);
        }

        /// <summary>
        /// Method to iterate the claims roles and save in the repository.
        /// </summary>
        /// <returns>Task</returns>
        private async Task SaveIdentityAttributes()
        {
            var claimsPrincipal = User.Identities.FirstOrDefault();

            if (claimsPrincipal != null)
            {
                var id = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimName.Sub)?.Value;
                var displayName = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == DasClaimTypes.DisplayName)?.Value;
                var ukPrn = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == DasClaimTypes.Ukprn)?.Value;
                var email = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == ClaimName.Email)?.Value;

                claimsPrincipal.MapClaimToRoles();

                await _authenticationOrchestrator.SaveIdentityAttributes(id, ukPrn, displayName, email);
            }
        }
    }
}
