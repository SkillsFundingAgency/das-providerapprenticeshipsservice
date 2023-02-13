using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using AccountApiConfiguration = SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration.AccountApiConfiguration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    public static class ConfigurationServiceRegistrations
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IProviderAgreementStatusConfiguration>(configuration.Get<ProviderApprenticeshipsServiceConfiguration>());
            services.AddSingleton<IAccountApiConfiguration>(configuration.Get<AccountApiConfiguration>());
            services.AddSingleton<IAccountApiClient>(configuration.Get<AccountApiClient>());

            return services;
        }
    }
}
