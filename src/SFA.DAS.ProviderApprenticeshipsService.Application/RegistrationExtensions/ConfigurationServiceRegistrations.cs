using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;

public static class ConfigurationServiceRegistrations
{
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IBaseConfiguration>(configuration.Get<ProviderApprenticeshipsServiceConfiguration>());
        services.Configure<ProviderApprenticeshipsServiceConfiguration>(_=>configuration.Get<ProviderApprenticeshipsServiceConfiguration>());
        services.AddSingleton(cfg => cfg.GetService<IOptions<ProviderApprenticeshipsServiceConfiguration>>().Value);
        services.AddSingleton(configuration.Get<ProviderApprenticeshipsServiceConfiguration>());
        services.AddSingleton(configuration.Get<ProviderUrlConfiguration>());

        return services;
    }
}