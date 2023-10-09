using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using System.Net.Http;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    [ExcludeFromCodeCoverage]
    public static class TrainingProviderApiClientRegistration
    {
        public static IServiceCollection AddTrainingProviderApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TrainingProviderApiClientConfiguration>(c => configuration.GetSection("TrainingProviderApiClient").Bind(c));
            services.AddSingleton(cfg => cfg.GetService<IOptions<TrainingProviderApiClientConfiguration>>().Value);

            services.AddSingleton<ITrainingProviderApiClient>(s =>
            {
                var trainingProviderApiLogger = new LoggerFactory().CreateLogger<TrainingProviderApiClient>();
                var trainingProviderApiClientConfiguration = s.GetService<TrainingProviderApiClientConfiguration>();
                var httpClient = GetHttpClient(trainingProviderApiClientConfiguration, configuration);
                return new TrainingProviderApiClient(httpClient, trainingProviderApiClientConfiguration, trainingProviderApiLogger);
            });
            return services;
        }

        private static HttpClient GetHttpClient(TrainingProviderApiClientConfiguration apiClientConfiguration, IConfiguration config)
        {
            var httpClientBuilder = !config.IsLocal()
                ? new HttpClientBuilder()
                : new HttpClientBuilder().WithBearerAuthorisationHeader(new ManagedIdentityTokenGenerator(apiClientConfiguration));
            return httpClientBuilder.Build();
        }
    }
}
