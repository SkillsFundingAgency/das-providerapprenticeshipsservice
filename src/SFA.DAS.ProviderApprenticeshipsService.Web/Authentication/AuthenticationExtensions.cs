using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using Microsoft.AspNetCore.Authentication.WsFederation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authentication
{
    public static class AuthenticationExtensions
    {
        public static void AddAndConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();
            var providerCommitmentsLogger = serviceProvider.GetService<IProviderCommitmentsLogger>();
            var accountOrchestrator = serviceProvider.GetService<IAuthenticationOrchestrator>();
            var idamsMetadata = configuration.GetSection("IdamsADFSMetadata").Value;
            var wtRealm = configuration.GetSection("IdamsRealm").Value;

            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme =
                    CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultSignInScheme =
                    CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme =
                    WsFederationDefaults.AuthenticationScheme;
            })
                .AddWsFederation(options =>
                {
                    options.MetadataAddress = idamsMetadata;
                    options.Wtrealm = wtRealm;
                    options.Events.OnSecurityTokenValidated = async (context) =>
                    {
                        await SecurityTokenValidated(context.HttpContext, context.Principal, providerCommitmentsLogger, accountOrchestrator);
                    };
                })
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = $"SFA.DAS.ProviderApprenticeshipsService.Web.Auth";
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
                });
        }

        private static async Task SecurityTokenValidated(HttpContext httpContext, ClaimsPrincipal principal, IProviderCommitmentsLogger logger,
           IAuthenticationOrchestrator orchestrator)
        {
            logger.Info("SecurityTokenValidated notification called");

            var id = principal.Claims.FirstOrDefault(claim => claim.Type == DasClaimTypes.Upn)?.Value;
            var displayName = principal.Claims.FirstOrDefault(claim => claim.Type == DasClaimTypes.DisplayName)?.Value;
            var ukprn = principal.Claims.FirstOrDefault(claim => claim.Type == DasClaimTypes.Ukprn)?.Value;
            var email = principal.Claims.FirstOrDefault(claim => claim.Type == DasClaimTypes.Email)?.Value;

            httpContext.Items.Add(ClaimsIdentity.DefaultNameClaimType, id);
            httpContext.Items.Add(DasClaimTypes.DisplayName, displayName);
            httpContext.Items.Add(DasClaimTypes.Ukprn, ukprn);
            httpContext.Items.Add(DasClaimTypes.Email, email);

            long parsedUkprn;

            if (!long.TryParse(ukprn, out parsedUkprn))
            {
                logger.Info($"Unable to parse Ukprn \"{ukprn}\" from claims for user \"{id}\"");
                return;
            }

            principal.Identities.First().MapClaimToRoles();

            await orchestrator.SaveIdentityAttributes(id, parsedUkprn, displayName, email);

            return;
        }
    }
}