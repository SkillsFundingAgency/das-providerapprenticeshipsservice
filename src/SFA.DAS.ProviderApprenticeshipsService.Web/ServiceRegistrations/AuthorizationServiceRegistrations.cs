using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Caching;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations;

public static class AuthorizationServiceRegistrations
{
    public static IServiceCollection AddAuthorization<T>(this IServiceCollection services) where T : class, IAuthorizationContextProvider
    {
        services.AddLogging()
            .AddMemoryCache()
            .AddScoped<IAuthorizationContextProvider>(p => new AuthorizationContextCache(p.GetService<T>()))
            .AddScoped<IAuthorizationService, AuthorizationService>()
            .AddScoped<T>()
            .AddScoped(p => p.GetService<IAuthorizationContextProvider>().GetAuthorizationContext());
        
        return services;
    }
}