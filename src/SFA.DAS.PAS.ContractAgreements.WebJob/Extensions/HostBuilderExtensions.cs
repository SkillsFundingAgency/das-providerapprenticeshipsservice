using System;
using System.Reflection;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.PAS.ContractAgreements.WebJob.Configuration;
using SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.PAS.ContractAgreements.WebJob.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((context, services) =>
        {
            services.Configure<ContractFeedConfiguration>(
                context.Configuration.GetSection(ConfigurationKeys.ContractAgreements));
            services.AddSingleton<IContractFeedConfiguration>(isp =>
                isp.GetService<IOptions<ContractFeedConfiguration>>().Value);
            services.AddSingleton<IBaseConfiguration>(
                isp => isp.GetService<IOptions<ContractFeedConfiguration>>().Value);
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
            services.AddLogging();
        });

        return hostBuilder;
    }

    public static IHostBuilder AddConfiguration(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureAppConfiguration((context, builder) =>
        {
            var environment = context.HostingEnvironment.EnvironmentName;

            if (environment.Equals("LOCAL") || environment.Equals("AT") || environment.Equals("TEST"))
            {
                PopulateSystemDetails(environment);
            }

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
        {
            environment = Environment.GetEnvironmentVariable(EnvironmentVariableNames.EnvironmentName);
        }

        return hostBuilder.UseEnvironment(environment);
    }

    private static void PopulateSystemDetails(string envName)
    {
        SystemDetails.EnvironmentName = envName;
        
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        
        if (version != null)
        {
            SystemDetails.VersionNumber = version.ToString();
        }
    }
}