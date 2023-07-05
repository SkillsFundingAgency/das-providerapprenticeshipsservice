using Microsoft.Extensions.Configuration;
using SFA.DAS.Api.Common.AppStart;
using SFA.DAS.Api.Common.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.Api.Common.Infrastructure.Configuration;
using SFA.DAS.PAS.Account.Api.Configuration;

namespace SFA.DAS.PAS.Account.Api.Authentication;

public static class AuthenticationExtensions
{
    private const string BasicAuthScheme = "BasicAuthentication";
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration config)
    {
        if (config.IsDevOrLocal())
        {
            services
                .AddAuthentication(BasicAuthScheme)
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthScheme, null);
        }
        else
        {
            var azureAdConfiguration = config
                .GetSection(ConfigurationKeys.AzureActiveDirectoryApiConfiguration)
                .Get<AzureActiveDirectoryConfiguration>();

            var policies = new Dictionary<string, string> { { PolicyNames.Default, RoleNames.Default } };
            
            services.AddAuthentication(azureAdConfiguration, policies);
            services.AddSingleton<IClaimsTransformation, AzureAdScopeClaimTransformation>();
        }

        return services;
    }
}