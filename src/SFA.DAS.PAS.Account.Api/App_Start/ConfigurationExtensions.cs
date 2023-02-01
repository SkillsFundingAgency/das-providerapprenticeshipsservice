using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SFA.DAS.PAS.Account.Api;

public static class ConfigurationExtensions
{
    public static IConfigurationRoot LoadConfiguration(this IConfiguration config)
    {
        var configBuilder = new ConfigurationBuilder()
            .AddConfiguration(config)
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();

        if (!IsDev(config))
        {
#if DEBUG
            configBuilder
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.Development.json", true);
#endif

            configBuilder.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = config["ConfigNames"]?.Split(",");
                    options.StorageConnectionString = config["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = config["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                }
            );
        }

        return configBuilder.Build();
    }

    public static void AddConfiguration(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IProviderAgreementStatusConfiguration>(config.Get<ProviderApprenticeshipsServiceConfiguration>());
    }

    public static bool IsDev(this IConfiguration configuration)
    {
        var isDev = ((configuration["EnvironmentName"]?.StartsWith("DEV", StringComparison.CurrentCultureIgnoreCase)) ?? false);
        var isDevelopment = ((configuration["EnvironmentName"]?.StartsWith("Development", StringComparison.CurrentCultureIgnoreCase)) ?? false);

        return (isDev || isDevelopment);
    }

    public static bool IsLocal(this IConfiguration configuration)
    {
        return configuration["EnvironmentName"]?.StartsWith("LOCAL", StringComparison.CurrentCultureIgnoreCase) ?? false;
    }
}