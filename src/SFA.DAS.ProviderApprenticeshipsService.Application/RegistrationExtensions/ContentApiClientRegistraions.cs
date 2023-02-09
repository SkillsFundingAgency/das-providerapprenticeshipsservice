using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Http;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using System.Net.Http;
using Microsoft.Extensions.Options;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    public static class ContentApiClientRegistraions
    {
        public static IServiceCollection AddContentApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ContentClientApiConfiguration>(c => configuration.GetSection("ContentApi").Bind(c));
            services.AddSingleton<IContentApiConfiguration>(cfg => cfg.GetService<IOptions<ContentClientApiConfiguration>>().Value);

            services.AddSingleton<IContentApiClient>(s =>
            {
                var contentApiConfig = s.GetService<IContentApiConfiguration>();
                var httpClient = GetHttpClient(contentApiConfig);

                return new ContentApiClient(httpClient, contentApiConfig);
            });

            return services;
        }

        private static HttpClient GetHttpClient(IContentApiConfiguration config)
        {
            HttpClient httpClient = new HttpClientBuilder()
                    .WithBearerAuthorisationHeader(new ManagedIdentityTokenGenerator(config))
                    // .WithHandler(new RequestIdMessageRequestHandler())
                    // .WithHandler(new SessionIdMessageRequestHandler())
                    .WithDefaultHeaders()
                    .Build();

            return httpClient;
        }
    }
}
