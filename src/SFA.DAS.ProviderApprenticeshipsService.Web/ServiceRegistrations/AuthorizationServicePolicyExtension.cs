using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Caching;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations;

public static class AuthorizationServicePolicyExtension
{
    private const string ProviderDaa = "DAA";
    private const string ProviderDab = "DAB";
    private const string ProviderDac = "DAC";
    private const string ProviderDav = "DAV";

    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        services.AddSingleton<IActionContextAccessorWrapper, ActionContextAccessorWrapper>();

        services.AddAuthorization<AuthorizationContextProvider>();
        services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();

        services.AddSingleton(provider => provider.GetService<IAuthorizationContextProvider>().GetAuthorizationContext());
        services.Decorate<IAuthorizationContextProvider, AuthorizationContextCache>();
        services.Decorate<IAuthorizationHandler, AuthorizationResultLogger>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddSingleton<ITrainingProviderAuthorizationHandler, TrainingProviderAuthorizationHandler>();
        services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, TrainingProviderAllRolesAuthorizationHandler>();

        return services;
    }
    
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.AuthenticatedUser, policy => { policy.RequireAuthenticatedUser(); });
            options.AddPolicy(PolicyNames.RequireDasPermissionRole, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(DasClaimTypes.Service, ProviderDaa, ProviderDab, ProviderDac, ProviderDav);
                policy.Requirements.Add(new TrainingProviderAllRolesRequirement()); //Policy requirement to check if the signed provider is a Main or Employer Profile.
            });
        });

        return services;
    }
}