using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.DependencyResolution.Microsoft;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.Authorization.ProviderPermissions.Handlers;
using SFA.DAS.Authorization.Caching;
using SFA.DAS.Authorization.Logging;
using SFA.DAS.Authorization.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization
{
    public static class AuthorizationServicePolicyExtension
    {
        public static void AddAuthorizationServicePolicies(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    PolicyNames
                        .RequireAuthenticatedUser
                    , policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });

                options.AddPolicy(
                    PolicyNames
                        .RequireDasPermissionRole
                    , policy =>
                    {
                        policy.RequireRole(RoleNames.DasPermission);
                    });
            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IActionContextAccessorWrapper, ActionContextAccessorWrapper>();

            // ***Deafult Registry***
            services.AddAuthorization<AuthorizationContextProvider>();
            services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();

            // ***AuthorisationRegistry***
            services.AddSingleton<IAuthorizationContext>(_ => _.GetService<IAuthorizationContextProvider>().GetAuthorizationContext());
            services.AddSingleton<IAuthorizationContextProvider, DefaultAuthorizationContextProvider>();
            services.Decorate<IAuthorizationContextProvider, AuthorizationContextCache>();
            services.Decorate<IAuthorizationHandler, AuthorizationResultLogger>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddSingleton<IDefaultAuthorizationHandler, DefaultAuthorizationHandler>();
            //services.AddScoped<IAuthorizationService, AuthorizationServiceWithDefaultHandler>();
        }
    }
}
