using SFA.DAS.Authorization.Caching;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.DependencyResolution.Microsoft;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.Authorization.Logging;
using SFA.DAS.Authorization.ProviderFeatures.Handlers;
using SFA.DAS.Authorization.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;

public static class AuthorizationServicePolicyExtension
{
    public static void AddAuthorizationServicePolicies(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.RequireDasPermissionRole, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(RoleNames.DasPermission);
            });
        });

        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddSingleton<IActionContextAccessorWrapper, ActionContextAccessorWrapper>();

        services.AddAuthorization<AuthorizationContextProvider>();
        services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();

        services.AddSingleton(_ => _.GetService<IAuthorizationContextProvider>().GetAuthorizationContext());
        //services.AddSingleton<IAuthorizationContextProvider, DefaultAuthorizationContextProvider>();
        services.Decorate<IAuthorizationContextProvider, AuthorizationContextCache>();
        services.Decorate<IAuthorizationHandler, AuthorizationResultLogger>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddSingleton<IDefaultAuthorizationHandler, DefaultAuthorizationHandler>();
    }
}