using Microsoft.Extensions.Caching.Memory;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Caching;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Logging;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        return services.AddAuthorization<DefaultAuthorizationContextProvider>();
    }

    public static IServiceCollection AddAuthorization<T>(this IServiceCollection services) where T : class, IAuthorizationContextProvider
    {
        services.AddLogging()
            .AddMemoryCache()
            .AddScoped<IAuthorizationContextProvider>(p => new AuthorizationContextCache(p.GetService<T>()))
            .AddScoped<IAuthorizationService, AuthorizationService>()
            .AddScoped<IDefaultAuthorizationHandler, DefaultAuthorizationHandler>()
            .AddScoped<T>()
            .AddScoped(p => p.GetService<IAuthorizationContextProvider>().GetAuthorizationContext());

        services.Decorate<IAuthorizationService, AuthorizationServiceWithDefaultHandler>();

        return services;
    }

    public static IServiceCollection AddAuthorizationHandler<T>(this IServiceCollection services, bool enableAuthorizationResultCache = false) where T : class, IAuthorizationHandler
    {
        return services.AddScoped<T>()
            .AddScoped(p =>
            {
                var authorizationHandler = (IAuthorizationHandler)p.GetService(typeof(T));
                var authorizationResultCacheConfigurationProviders = p.GetServices<IAuthorizationResultCacheConfigurationProvider>();
                var memoryCache = p.GetService<IMemoryCache>();
                var logger = p.GetService<ILogger<AuthorizationResultLogger>>();

                if (enableAuthorizationResultCache)
                {
                    authorizationHandler = new AuthorizationResultCache(authorizationHandler, authorizationResultCacheConfigurationProviders, memoryCache);
                }

                authorizationHandler = new AuthorizationResultLogger(authorizationHandler, logger);

                return authorizationHandler;
            });
    }
}