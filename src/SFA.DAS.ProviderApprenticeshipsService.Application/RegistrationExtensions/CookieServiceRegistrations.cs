using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.CookieStorageService;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;

public static class CookieServiceRegistrations
{
    public static IServiceCollection AddCookieStorageService(this IServiceCollection services)
    {
        services.AddScoped(typeof(ICookieService<>), typeof(HttpCookieService<>));
        services.AddScoped(typeof(ICookieStorageService<>), typeof(CookieStorageService<>));

        return services;
    }
}