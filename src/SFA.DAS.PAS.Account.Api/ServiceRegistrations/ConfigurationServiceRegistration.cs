using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Configurations;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.PAS.Account.Api.ServiceRegistrations;

public static class ConfigurationServiceRegistrations
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IBaseConfiguration>(configuration.Get<PasAccountApiConfiguration>());
        services.AddSingleton<IPasAccountApiConfiguration>(configuration.Get<PasAccountApiConfiguration>());

        return services;
    }
}