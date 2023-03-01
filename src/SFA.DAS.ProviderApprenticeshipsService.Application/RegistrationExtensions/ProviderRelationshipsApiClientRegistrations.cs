using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderRelationships.Api.Client.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Http;
using System.Net.Http;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions
{
    public static class ProviderRelationshipsApiClientRegistrations
    {
        public static IServiceCollection AddProviderRelationshipsApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ProviderRelationshipsApiConfiguration>(c => configuration.GetSection("ProviderRelationshipsApi").Bind(c));

            var useStub = GetUseStubProviderRelationshipsSetting(configuration);
            if (useStub)
            {
                services.AddTransient<IProviderRelationshipsApiClient, StubProviderRelationshipsApiClient>();
            }
            else
            {
                services.AddSingleton<IProviderRelationshipsApiClient>(s =>
                {
                    var config = s.GetService<ProviderRelationshipsApiConfiguration>();
                    var restHttpClient = GetRestHttpClient(config);

                    return new ProviderRelationshipsApiClient(restHttpClient);
                });
            }

            return services;
        }

        private static RestHttpClient GetRestHttpClient(ProviderRelationshipsApiConfiguration config)
        {
            HttpClient httpClient = new HttpClientBuilder()
                    .WithBearerAuthorisationHeader(new ManagedIdentityTokenGenerator(config))
                    .WithDefaultHeaders()
                    .Build();

            return new RestHttpClient(httpClient);
        }

        private static bool GetUseStubProviderRelationshipsSetting(IConfiguration configuration)
        {
            var value = configuration.GetSection("UseStubProviderRelationships").Value;

            if (value == null)
            {
                return false;
            }

            if (!bool.TryParse(value, out var result))
            {
                return false;
            }

            return result;
        }
    }
}
