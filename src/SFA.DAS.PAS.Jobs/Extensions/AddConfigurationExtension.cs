using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Functions.Worker.Builder;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.PAS.Jobs.Extensions;

[ExcludeFromCodeCoverage]
public static class AddConfigurationExtension
{
    public static void AddConfiguration(this FunctionsApplicationBuilder builder)
    {
        builder.Configuration.AddAzureTableStorage(options =>
        {
            options.ConfigurationKeys = builder.Configuration["ConfigNames"].Split(',');
            options.StorageConnectionString = builder.Configuration["ConfigurationStorageConnectionString"];
            options.EnvironmentName = builder.Configuration["EnvironmentName"];
            options.PreFixConfigurationKeys = false;
        });
    }
}
