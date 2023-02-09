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

            // TBC if it can be removed: I dont think this is used but replacing:
            // c.Policies.Add(new ConfigurationPolicy<AccountApiConfiguration>("SFA.DAS.EmployerAccountAPI"));
            // in old IoC.cs
            services.AddSingleton<IAccountApiConfiguration>(configuration.Get<AccountApiConfiguration>());

            return services;
        }
    }
}
