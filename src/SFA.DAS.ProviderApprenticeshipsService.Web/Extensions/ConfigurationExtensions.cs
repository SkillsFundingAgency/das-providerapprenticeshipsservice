﻿using System.IO;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

public static class ConfigurationExtensions
{
    public static IConfiguration BuildDasConfiguration(this IConfiguration configuration)
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory());
#if DEBUG
        if (!configuration.IsDev())
        {
            configurationBuilder.AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.Development.json", true);
        }
#endif

        configurationBuilder.AddEnvironmentVariables();

        if (!configuration.IsDev())
        {
            configurationBuilder.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                    options.ConfigurationKeysRawJsonResult = new[] { "SFA.DAS.Encoding" };
                }
            );
        }

        return configurationBuilder.Build();
    }
}