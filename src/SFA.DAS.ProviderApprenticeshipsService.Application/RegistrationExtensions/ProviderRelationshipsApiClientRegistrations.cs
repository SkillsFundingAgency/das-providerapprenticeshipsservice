using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ProviderRelationships.Api.Client.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderRelationships.Api.Client;

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
                services.AddTransient<IProviderRelationshipsApiClient, ProviderRelationshipsApiClient>();
            }

            return services;
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
