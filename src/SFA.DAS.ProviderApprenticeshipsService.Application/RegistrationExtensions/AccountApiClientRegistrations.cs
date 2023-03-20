using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Account.Api.Client;
using AccountApiConfiguration = SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration.AccountApiConfiguration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    public static class AccountApiClientRegistrations
    {
        public static IServiceCollection AddAccountApiClient(this IServiceCollection services, IConfiguration configuration)
        {
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
