﻿using Microsoft.AspNetCore.Authentication.Cookies;
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
using SFA.DAS.DfESignIn.Auth.AppStart;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authentication
{
    public static class AuthenticationExtensions
    {
        private const string ClientName = "ProviderRoATP";
        private const string SignedOutCallbackPath = "/signout";

        public static void AddAndConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            
            var useDfESignIn = configuration.GetSection("RoatpCourseManagement:UseDfESignIn").Get<bool>();

            if (useDfESignIn)
            {
                services.AddAndConfigureDfESignInAuthentication(
                    configuration,
                    $"SFA.DAS.ProviderApprenticeshipsService.Web.Auth",
                    typeof(CustomServiceRole),
                    ClientName,
                    SignedOutCallbackPath);
            }
            else
            {
                var authenticationConfiguration = configuration.GetSection("ProviderIdams").Get<AuthenticationConfiguration>();

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
                        options.MetadataAddress = authenticationConfiguration.MetaDataAddress;
                        options.Wtrealm = authenticationConfiguration.WtRealm;
                        options.UseTokenLifetime = false;
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
                services
                    .AddOptions<WsFederationOptions>(WsFederationDefaults.AuthenticationScheme)
                    .Configure<IProviderCommitmentsLogger, IAuthenticationOrchestrator>((options, providerCommitmentsLogger, accountOrchestrator) =>
                    {
                        options.Events.OnSecurityTokenValidated = async (ctx) =>
                        {
                            await SecurityTokenValidated(ctx, providerCommitmentsLogger, accountOrchestrator);
                        };
                    });
            }
        }

        private static async Task SecurityTokenValidated(SecurityTokenValidatedContext ctx, IProviderCommitmentsLogger logger,
           IAuthenticationOrchestrator orchestrator)
        {
            logger.Info("SecurityTokenValidated notification called");

            var id = ctx.Principal.Claims.FirstOrDefault(claim => claim.Type == DasClaimTypes.Upn)?.Value;
            var displayName = ctx.Principal.Claims.FirstOrDefault(claim => claim.Type == DasClaimTypes.DisplayName)?.Value;
            var ukprn = ctx.Principal.Claims.FirstOrDefault(claim => claim.Type == DasClaimTypes.Ukprn)?.Value;
            var email = ctx.Principal.Claims.FirstOrDefault(claim => claim.Type == DasClaimTypes.Email)?.Value;

            ctx.HttpContext.Items.Add(ClaimsIdentity.DefaultNameClaimType, id);
            ctx.HttpContext.Items.Add(DasClaimTypes.DisplayName, displayName);
            ctx.HttpContext.Items.Add(DasClaimTypes.Ukprn, ukprn);
            ctx.HttpContext.Items.Add(DasClaimTypes.Email, email);

            ctx.Principal.Identities.First().MapClaimToRoles();

            await orchestrator.SaveIdentityAttributes(id, ukprn, displayName, email);
        }
    }
}