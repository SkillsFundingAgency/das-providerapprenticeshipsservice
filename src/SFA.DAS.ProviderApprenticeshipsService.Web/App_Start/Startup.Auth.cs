using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.WsFederation;
using Newtonsoft.Json;
using OpenAthens.Owin.Security.OpenIdConnect;
using Owin;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Api.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.App_Start;
using SFA.DAS.ProviderApprenticeshipsService.Web.Constants;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OpenIdConnectMessage = Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectMessage;
using WsFederationMessage = Microsoft.IdentityModel.Protocols.WsFederation.WsFederationMessage;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            var logger = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<IProviderCommitmentsLogger>();
            var dfESignInConfig = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<IDfESignInServiceConfiguration>();
            var providerApprenticeshipsServiceConfig = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<ProviderApprenticeshipsServiceConfiguration>();
            var authenticationOrchestrator = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<AuthenticationOrchestrator>();
            var accountOrchestrator = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<AccountOrchestrator>();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieManager = new SystemWebCookieManager()
            });

            // check if the DfESignIn Switch is available and enabled.
            if (providerApprenticeshipsServiceConfig != null && providerApprenticeshipsServiceConfig.UseDfESignIn)
            {
                var oidcRedirectUrl = ConfigurationManager.AppSettings["IdamsRealm"] + "sign-in";
                app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
                {
                    Authority = dfESignInConfig.DfEOidcConfiguration.BaseUrl,
                    ClientId = dfESignInConfig.DfEOidcConfiguration.ClientId,
                    ClientSecret = dfESignInConfig.DfEOidcConfiguration.Secret,
                    GetClaimsFromUserInfoEndpoint = true,
                    PostLogoutRedirectUri = oidcRedirectUrl,
                    RedirectUri = oidcRedirectUrl,
                    ResponseType = OpenIdConnectResponseType.Code,
                    Scope = "openid email profile organisation organisationid",
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        SecurityTokenValidated = async notification =>
                        {
                            await PopulateAccountsClaim(notification, logger, authenticationOrchestrator);
                        }
                    },
                });
            }
            else
            {
                app.UseWsFederationAuthentication(new WsFederationAuthenticationOptions
                {
                    Wtrealm = ConfigurationManager.AppSettings["IdamsRealm"],
                    MetadataAddress = ConfigurationManager.AppSettings["IdamsADFSMetadata"],
                    Notifications = new WsFederationAuthenticationNotifications
                    {
                        SecurityTokenValidated = notification => SecurityTokenValidated(notification, logger, authenticationOrchestrator, accountOrchestrator)
                    }
                });
            }
        }
        
        private async Task SecurityTokenValidated(SecurityTokenValidatedNotification<WsFederationMessage, WsFederationAuthenticationOptions> notification, IProviderCommitmentsLogger logger, 
            AuthenticationOrchestrator orchestrator, AccountOrchestrator accountOrchestrator)
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

        /// <summary>
        /// Method to populate/add the OpenIdConnect claims to the initial identity.
        /// </summary>
        /// <param name="notification">Security Token.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="orchestrator">Authentication Orchestrator.</param>
        /// <returns>Task.</returns>
        private static async Task PopulateAccountsClaim(SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification, IProviderCommitmentsLogger logger,
            AuthenticationOrchestrator orchestrator)
        {
            logger.Info("SecurityTokenValidated notification called");

            #region "READ THE CLAIMS FROM AUTHENTICATION TICKET"
            var userId = notification.AuthenticationTicket.Identity.GetClaimValue(ClaimName.NameIdentifier);
            var emailAddress = notification.AuthenticationTicket.Identity.GetClaimValue(ClaimName.Email);
            var displayName = $"{notification.AuthenticationTicket.Identity.GetClaimValue(ClaimName.GivenName)} {notification.AuthenticationTicket.Identity.GetClaimValue(ClaimName.Surname)}";
            var userOrganization = JsonConvert.DeserializeObject<Organisation>(notification.AuthenticationTicket.Identity.GetClaimValue(ClaimName.Organisation));
            #endregion

            var ukPrn = userOrganization.UkPrn != null ? Convert.ToInt64(userOrganization.UkPrn) : 0;

            // when the UserId and UserOrgId are available then fetch additional claims from DfESignIn Api Service.
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userOrganization.Id.ToString()))
                await DfEPublicApi(notification, userId, userOrganization.Id.ToString());

            System.Web.HttpContext.Current.Items.Add(ClaimsIdentity.DefaultNameClaimType, ukPrn);
            System.Web.HttpContext.Current.Items.Add(CustomClaimsIdentity.DisplayName, displayName);

            notification.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, ukPrn.ToString()));
            notification.AuthenticationTicket.Identity.AddClaim(new Claim(CustomClaimsIdentity.DisplayName, displayName));
            notification.AuthenticationTicket.Identity.AddClaim(new Claim(CustomClaimsIdentity.UkPrn, ukPrn.ToString()));

            notification.AuthenticationTicket.Identity.MapClaimToRoles();

            logger.Info("Identity information added to the user repository.");

            // store the identity information in user repository.
            await orchestrator.SaveIdentityAttributes(userId, ukPrn, displayName, emailAddress);
        }

        /// <summary>
        /// Method to get additional claims from DfESignIn Api Service.
        /// </summary>
        /// <param name="notification">Security Token.</param>
        /// <param name="userId">string User Identifier.</param>
        /// <param name="userOrgId">string User Organization Identifier.</param>
        /// <returns>Task.</returns>
        private static async Task DfEPublicApi(SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification, string userId, string userOrgId)
        {
            var dfESignInService = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<IDfESignInService>();
            
            // fetch the additional claims from DfESignIn Services.
            var apiServiceResponse = await dfESignInService.Get<ApiServiceResponse>(userId, userOrgId);

            if(apiServiceResponse != null)
            {
                var roleClaims = new List<Claim>();
                foreach (var role in apiServiceResponse.Roles.Where(role => role.Status.Id.Equals(1)))
                {
                    // add to current identity because you cannot have multiple identities
                    roleClaims.Add(new Claim(ClaimName.RoleCode, role.Code, ClaimTypes.Role, notification.Options.ClientId));
                    roleClaims.Add(new Claim(ClaimName.RoleId, role.Id.ToString(), ClaimTypes.Role, notification.Options.ClientId));
                    roleClaims.Add(new Claim(ClaimName.RoleName, role.Name, ClaimTypes.Role, notification.Options.ClientId));
                    roleClaims.Add(new Claim(ClaimName.RoleNumericId, role.NumericId.ToString(), ClaimTypes.Role, notification.Options.ClientId));

                    // add service role claim to initial identity.
                    notification.AuthenticationTicket.Identity.AddClaim(new Claim(DasClaimTypes.Service, role.Name));
                }

                // add service role claims to identity.
                notification.AuthenticationTicket.Identity.AddClaims(roleClaims);
            }
        }
    }
}
