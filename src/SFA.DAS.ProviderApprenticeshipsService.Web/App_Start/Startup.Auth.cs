using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.WsFederation;
using Owin;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.App_Start;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using System.Security.Claims;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            var logger = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<IProviderCommitmentsLogger>();

            var authenticationOrchestrator = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<AuthenticationOrchestrator>();
            var accountOrchestrator = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<AccountOrchestrator>();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                CookieManager = new SystemWebCookieManager()
            });

            var realm = ConfigurationManager.AppSettings["IdamsRealm"];
            var adfsMetadata = ConfigurationManager.AppSettings["IdamsADFSMetadata"];

            //todo: may need more options here
            var options = new WsFederationAuthenticationOptions
            {
                Wtrealm = realm,
                MetadataAddress = adfsMetadata,
                Notifications = new WsFederationAuthenticationNotifications
                {
                    SecurityTokenValidated = notification => SecurityTokenValidated(notification, logger, authenticationOrchestrator, accountOrchestrator)
                }
                //,Wreply = "?"
                //,SignOutWreply = "?"
            };

            app.UseWsFederationAuthentication(options);
        }
        
        private async Task SecurityTokenValidated(SecurityTokenValidatedNotification<WsFederationMessage, WsFederationAuthenticationOptions> notification, IProviderCommitmentsLogger logger, 
            AuthenticationOrchestrator orchestrator, AccountOrchestrator accountOrchestrator)
        {
            logger.Info("SecurityTokenValidated notification called");

            var identity = notification.AuthenticationTicket.Identity;
            
            identity.AddClaim(new Claim(DasClaimTypes.Upn, "isp\\10005077s", "string"));
            identity.AddClaim(new Claim(DasClaimTypes.DisplayName, "isp 10005077s", "string"));
            identity.AddClaim(new Claim(DasClaimTypes.Ukprn, "10005077", "string"));
            identity.AddClaim(new Claim(DasClaimTypes.Email, "10005077@gov.uk", "string"));
            identity.AddClaim(new Claim("http://schemas.portal.com/service", "DAA", "string"));

            var id = identity.Claims.FirstOrDefault(claim => claim.Type == (DasClaimTypes.Upn))?.Value;
            var displayName = identity.Claims.FirstOrDefault(claim => claim.Type == (DasClaimTypes.DisplayName))?.Value;
            var ukprn = identity.Claims.FirstOrDefault(claim => claim.Type == (DasClaimTypes.Ukprn))?.Value;
            var email = identity.Claims.FirstOrDefault(claim => claim.Type == (DasClaimTypes.Email))?.Value;
            
            long parsedUkprn;
            if (!long.TryParse(ukprn, out parsedUkprn))
            {
                logger.Info($"Unable to parse Ukprn \"{ukprn}\" from claims for user \"{id}\"");
                return;
            }

            var showReservations = await accountOrchestrator.ProviderHasPermission(parsedUkprn, Operation.CreateCohort);

            identity.AddClaim(new Claim(DasClaimTypes.ShowReservations, showReservations.ToString(), "bool"));

            await orchestrator.SaveIdentityAttributes(id, parsedUkprn, displayName, email);
        }
    }
}
