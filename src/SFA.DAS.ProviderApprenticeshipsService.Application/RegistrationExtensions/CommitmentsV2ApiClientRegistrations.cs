using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Http;
using System.Net.Http;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;

public static class CommitmentsV2ApiClientRegistrations
{
    public static IServiceCollection AddCommitmentsV2ApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CommitmentsApiClientV2Configuration>(c => configuration.GetSection("CommitmentsApiClientV2").Bind(c));
        services.AddSingleton(cfg => cfg.GetService<IOptions<CommitmentsApiClientV2Configuration>>().Value);

        services.AddSingleton<ICommitmentsV2ApiClient>(s =>
        {
            var commitmentsV2ApiLogger = new LoggerFactory().CreateLogger<CommitmentsV2ApiClient>();
            var commitmentsV2Config = s.GetService<CommitmentsApiClientV2Configuration>();
            var httpClient = GetHttpV2Client(commitmentsV2Config, configuration);

            return new CommitmentsV2ApiClient(httpClient, commitmentsV2Config, commitmentsV2ApiLogger);
        });

        return services;
    }

    private static HttpClient GetHttpV2Client(CommitmentsApiClientV2Configuration commitmentsV2Config, IConfiguration config)
    {
        var httpClientBuilder = config.IsLocal()
            ? new HttpClientBuilder()
            : new HttpClientBuilder().WithBearerAuthorisationHeader(new ManagedIdentityTokenGenerator(commitmentsV2Config));

        return httpClientBuilder
            .WithDefaultHeaders()
            .Build();
    }
}