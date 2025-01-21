using Azure.Identity;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Logging.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.PAS.ContractAgreements.WebJob.Configuration;
using SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.PAS.ContractAgreements.WebJob.Interfaces;
using SFA.DAS.PAS.ContractAgreements.WebJob.ScheduledJobs;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Configurations;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using System;
using System.Reflection;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((context, services) =>
        {
            services.Configure<ContractFeedConfiguration>(context.Configuration.GetSection(ConfigurationKeys.ContractAgreements));
            services.AddSingleton<IContractFeedConfiguration>(isp => isp.GetService<IOptions<ContractFeedConfiguration>>().Value);
            services.AddSingleton<IBaseConfiguration>(isp => isp.GetService<IOptions<ContractFeedConfiguration>>().Value);
            services.AddSingleton<IConfidentialClientApplication, ConfidentialClientApplication>();

            services.AddSingleton(new ChainedTokenCredential(
                new ManagedIdentityCredential(),
                new AzureCliCredential())
            );

            services.AddTransient<ICurrentDateTime, CurrentDateTime>();
            services.AddTransient<IContractFeedProcessorHttpClient, ContractFeedProcessorHttpClient>();
            services.AddTransient<IContractFeedEventValidator, ContractFeedEventValidator>();
            services.AddTransient<IContractFeedReader, ContractFeedReader>();
            services.AddTransient<IContractDataProvider, ContractFeedProcessor>();
            services.AddTransient<IProviderAgreementStatusRepository, ProviderAgreementStatusRepository>();
            services.AddTransient<IProviderAgreementStatusService, ProviderAgreementStatusService>();
            services.AddTransient<UpdateAgreementStatusJob>();
            services.AddLogging()
                .AddTelemetryRegistration((IConfigurationRoot)context.Configuration)
                .AddApplicationInsightsTelemetry();
        });

        return hostBuilder;
    }

    public static IHostBuilder ConfigureDasWebJobs(this IHostBuilder builder)
    {
        builder.ConfigureWebJobs(b => { b.AddTimers(); });

#pragma warning disable 618
        builder.ConfigureServices(s => s.AddSingleton<IWebHookProvider>(p => null));
#pragma warning restore 618

        return builder;
    }

    public static IHostBuilder AddConfiguration(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureAppConfiguration((context, builder) =>
        {
            var environment = context.HostingEnvironment.EnvironmentName;

            if (environment.Equals("LOCAL") || environment.Equals("AT") || environment.Equals("TEST"))
                PopulateSystemDetails(environment);

            builder.AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddAzureTableStorage(ConfigurationKeys.ContractAgreements)
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

    private static void PopulateSystemDetails(string envName)
    {
        SystemDetails.EnvironmentName = envName;

        var version = Assembly.GetExecutingAssembly().GetName().Version;

        if (version != null) SystemDetails.VersionNumber = version.ToString();
    }

    public static IHostBuilder ConfigureDasLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureLogging((context, loggingBuilder) =>
        {
            var connectionString = context.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
            if (!string.IsNullOrEmpty(connectionString))
            {
                loggingBuilder.AddApplicationInsightsWebJobs(o => o.ConnectionString = connectionString);
                loggingBuilder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, Microsoft.Extensions.Logging.LogLevel.Information);
                loggingBuilder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", Microsoft.Extensions.Logging.LogLevel.Information);
            }

            loggingBuilder.AddConsole();
        });

        return hostBuilder;
    }
}