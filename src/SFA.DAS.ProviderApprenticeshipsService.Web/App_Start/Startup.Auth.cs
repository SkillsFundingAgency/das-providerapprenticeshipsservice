using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.IdentityModel.Protocols;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.App_Start;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using System.Security.Claims;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public partial class StartupAuth
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            var logger = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<IProviderCommitmentsLogger>();

            var authenticationOrchestrator = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<AuthenticationOrchestrator>();

            /* REPLACED
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                CookieManager = new SystemWebCookieManager()
            });
            */

            var options = new WsFederationAuthenticationOptions
            {
                Wtrealm = ConfigurationManager.AppSettings["IdamsRealm"],
                MetadataAddress = ConfigurationManager.AppSettings["IdamsADFSMetadata"],
                Notifications = new WsFederationAuthenticationNotifications
                {
                    SecurityTokenValidated = notification => SecurityTokenValidated(notification, logger, authenticationOrchestrator)
                }
            };

            app.UseWsFederationAuthentication(options);
        }
        
        private async Task SecurityTokenValidated(SecurityTokenValidatedNotification<WsFederationMessage, WsFederationAuthenticationOptions> notification, IProviderCommitmentsLogger logger, 
            AuthenticationOrchestrator orchestrator)
        {
            logger.Info("SecurityTokenValidated notification called");

            var identity = notification.AuthenticationTicket.Identity;
         
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

            identity.MapClaimToRoles();

            await orchestrator.SaveIdentityAttributes(id, parsedUkprn, displayName, email);
        }
    }
}
