using System.IO;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.PAS.Account.Api;

public static class ConfigurationExtensions
{
    public static IConfiguration BuildDasConfiguration(this IConfiguration configuration)
    {
        var config = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory());

#if DEBUG
        if (!configuration.IsDev())
        {
            config.AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.Development.json", true);
        }
#endif

        config.AddEnvironmentVariables();

        config.AddAzureTableStorage(options =>
        {
            options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
            options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
            options.EnvironmentName = configuration["EnvironmentName"];
            options.PreFixConfigurationKeys = false;
        }
        );

        return config.Build();
    }

    private static bool IsDev(this IConfiguration configuration)
    {
        var isDev = configuration["EnvironmentName"]?.StartsWith("DEV", StringComparison.CurrentCultureIgnoreCase) ?? false;
        var isDevelopment = configuration["EnvironmentName"]?.StartsWith("Development", StringComparison.CurrentCultureIgnoreCase) ?? false;

        return isDev || isDevelopment;
    }

    private static bool IsLocal(this IConfiguration configuration)
    {
        return configuration["EnvironmentName"]?.StartsWith("LOCAL", StringComparison.CurrentCultureIgnoreCase) ?? false;
    }

    public static bool IsDevOrLocal(this IConfiguration configuration)
    {
        return IsDev(configuration) || IsLocal(configuration);
    }
}