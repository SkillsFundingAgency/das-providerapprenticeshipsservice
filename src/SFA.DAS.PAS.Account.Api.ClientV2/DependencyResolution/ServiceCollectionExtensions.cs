using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.Account.Api.ClientV2.Configuration;
using SFA.DAS.AutoConfiguration.DependencyResolution;

namespace SFA.DAS.PAS.Account.Api.ClientV2.DependencyResolution
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPasAccountApiClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoConfiguration();

            services.AddSingleton(configuration.Get<PasAccountApiClientV2Configuration>()); // add SFA.DAS.PasAccountApiClientV2_1.0.json

            services.AddSingleton<IPasAccountApiClient>(s =>
            {
                var pasConfig = s.GetService<PasAccountApiClientV2Configuration>();
                var loggerFactory = s.GetService<ILoggerFactory>();
                var apiClientfactory = new PasAccountApiClientFactory(pasConfig, loggerFactory);
                return apiClientfactory.CreateClient();
            });

            return services;
        }
    }
}