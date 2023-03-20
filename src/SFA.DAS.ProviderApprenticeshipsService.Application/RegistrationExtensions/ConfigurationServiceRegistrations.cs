using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using AccountApiConfiguration = SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration.AccountApiConfiguration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    public static class ConfigurationServiceRegistrations
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddSingleton(configuration.Get<ProviderApprenticeshipsServiceConfiguration>());
            services.AddSingleton(configuration.Get<ProviderUrlConfiguration>());

            // I assumed that this config comes from SFA.DAS.EmployerAccountAPI because IAccountApiClient is used in EmployerAccountService
            // butTO BE CONFIRMED
            services.AddSingleton<IAccountApiConfiguration>(configuration.Get<AccountApiConfiguration>()); 
            services.AddSingleton<IAccountApiClient>(s =>
            {
                var accountApiConfig = s.GetService<IAccountApiConfiguration>();
                return new AccountApiClient(accountApiConfig);
            });

            return services;
        }
    }
}
