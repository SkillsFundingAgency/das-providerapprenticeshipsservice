using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Http;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using System.Net.Http;
using Microsoft.Extensions.Options;
using SFA.DAS.Http.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Configurations;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;

public static class ContentApiClientRegistrations
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

    private static HttpClient GetHttpClient(IManagedIdentityClientConfiguration config)
    {
        var httpClient = new HttpClientBuilder()
            .WithBearerAuthorisationHeader(new ManagedIdentityTokenGenerator(config))
            .WithDefaultHeaders()
            .Build();

        return httpClient;
    }
}