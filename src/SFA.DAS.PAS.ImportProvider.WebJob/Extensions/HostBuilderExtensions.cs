using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.PAS.ImportProvider.WebJob.Configuration;
using SFA.DAS.PAS.ImportProvider.WebJob.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Configurations;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.ImportProvider.WebJob.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((context, services) =>
        {
            services.Configure<ProviderApprenticeshipsServiceConfiguration>(context.Configuration.GetSection(ConfigurationKeys.ProviderApprenticeshipsService));
            
            services.AddSingleton<IBaseConfiguration>(isp => isp.GetService<IOptions<ProviderApprenticeshipsServiceConfiguration>>().Value);

            services.Configure<CommitmentsApiClientV2Configuration>(c => context.Configuration.GetSection(ConfigurationKeys.CommitmentsApiClientV2).Bind(c));
            
            services.AddSingleton(cfg => cfg.GetService<IOptions<CommitmentsApiClientV2Configuration>>().Value);
            services.AddHttpClient<ICommitmentsV2ApiClient, CommitmentsV2ApiClient>();

            services.AddSingleton(new ChainedTokenCredential(
                new ManagedIdentityCredential(),
                new AzureCliCredential())
            );

            services.AddTransient<IProviderRepository, ProviderRepository>();
            services.AddTransient<IImportProviderService, ImportProviderService>();
            services.AddLogging();
        });

        return hostBuilder;
    }

    public static IHostBuilder AddConfiguration(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureAppConfiguration((context, builder) =>
        {
            var environment = context.HostingEnvironment.EnvironmentName;

            builder.AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environment}.json", true, true)
                .AddAzureTableStorage(ConfigurationKeys.ProviderApprenticeshipsService)
                .AddEnvironmentVariables();
        });
    }

    public static IHostBuilder UseDasEnvironment(this IHostBuilder hostBuilder)
    {
        var environment = Environment.GetEnvironmentVariable("DASENV");
        if (string.IsNullOrEmpty(environment))
            environment = Environment.GetEnvironmentVariable(EnvironmentVariableNames.EnvironmentName);

        return hostBuilder.UseEnvironment(environment);
    }
    
    public static IHostBuilder ConfigureDasLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureLogging((context, loggingBuilder) =>
        {
            var connectionString = context.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
            if (!string.IsNullOrEmpty(connectionString))
            {
                loggingBuilder.AddApplicationInsightsWebJobs(o => o.ConnectionString = connectionString);
            }

            loggingBuilder.AddConsole();
        });

        return hostBuilder;
    }
}