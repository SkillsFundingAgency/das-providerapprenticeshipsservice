using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.PAS.Account.Api.ServiceRegistrations
{
    public static class ConfigurationServiceRegistrations
    {
        public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IBaseConfiguration>(configuration.Get<PasAccountApiConfiguration>());
            services.AddSingleton<IPasAccountApiConfiguration>(configuration.Get<PasAccountApiConfiguration>());

            return services;
        }
    }
}
