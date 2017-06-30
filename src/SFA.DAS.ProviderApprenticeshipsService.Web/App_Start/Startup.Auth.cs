using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.WsFederation;
using NLog;
using Owin;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.App_Start;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            var logger = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<IProviderCommitmentsLogger>();

            var authenticationOrchestrator = StructuremapMvc.StructureMapDependencyScope
                .Container.GetInstance<AuthenticationOrchestrator>();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                CookieManager = new SystemWebCookieManager()
            });

            var realm = CloudConfigurationManager.GetSetting("IdamsRealm");
            var adfsMetadata = CloudConfigurationManager.GetSetting("IdamsADFSMetadata");

            //todo: may need more options here
            var options = new WsFederationAuthenticationOptions
            {
                Wtrealm = realm,
                MetadataAddress = adfsMetadata,
                Notifications = new WsFederationAuthenticationNotifications
                {
                    SecurityTokenValidated = notification => SecurityTokenValidated(notification, logger, authenticationOrchestrator)
                }
                //,Wreply = "?"
                //,SignOutWreply = "?"
            };

            app.UseWsFederationAuthentication(options);
        }
        
        private async Task SecurityTokenValidated(SecurityTokenValidatedNotification<WsFederationMessage, WsFederationAuthenticationOptions> notification, IProviderCommitmentsLogger logger, AuthenticationOrchestrator orchestrator)
        {
            logger.Info("SecurityTokenValidated notification called");

            var identity = notification.AuthenticationTicket.Identity;

            var id = identity.Claims.FirstOrDefault(claim => claim.Type == (DasClaimTypes.Upn))?.Value;
            var displayName = identity.Claims.FirstOrDefault(claim => claim.Type == (DasClaimTypes.DisplayName))?.Value;
            var ukprn = identity.Claims.FirstOrDefault(claim => claim.Type == (DasClaimTypes.Ukprn))?.Value;
            var email = identity.Claims.FirstOrDefault(claim => claim.Type == (DasClaimTypes.Email))?.Value;

            logger.Info($"Captured claims for \"{id}\" - ukprn:\"{ukprn}\", displayname:\"{displayName}\", email: \"{email}\"");

            await orchestrator.SaveIdentityAttributes(id, ukprn, displayName, email);
        }
    }
}
