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


        if (!config["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {

#if DEBUG
            configBuilder
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.Development.json", true);
#endif

            configBuilder.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = config["ConfigNames"].Split(",");
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
        services.Configure<ProviderApprenticeshipsServiceConfiguration>(config.GetSection(nameof(ProviderApprenticeshipsServiceConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ProviderApprenticeshipsServiceConfiguration>>().Value);
        services.AddSingleton<IProviderAgreementStatusConfiguration>(cfg =>
            cfg.GetService<IOptions<ProviderApprenticeshipsServiceConfiguration>>().Value);
        //ProviderApprenticeshipsServiceConfiguration

    }
}