using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.Account.Api.ClientV2.Configuration;

namespace SFA.DAS.PAS.Account.Api.ClientV2.DependencyResolution
{
    public static class PasAccountApiClientRegistry
    {
        public static IServiceCollection AddPasAccountApiClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration.Get<PasAccountApiClientConfiguration>()); // CONFIG TO BE ADDED FOR PASAccountApi

            services.AddSingleton<IPasAccountApiClient>(s =>
            {
                var pasConfig = s.GetService<PasAccountApiClientConfiguration>();
                var loggerFactory = s.GetService<ILoggerFactory>();
                var apiClientfactory = new PasAccountApiClientFactory(pasConfig, loggerFactory);
                return apiClientfactory.CreateClient();
            });

            return services;
        }
    }
}