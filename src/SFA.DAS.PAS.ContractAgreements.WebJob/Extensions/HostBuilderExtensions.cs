using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.PAS.ContractAgreements.WebJob.Configuration;
using SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.PAS.ContractAgreements.WebJob.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using System;
using System.Reflection;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<IContractFeedConfiguration>(context.Configuration.Get<ContractFeedConfiguration>());

                services.AddTransient<ICurrentDateTime, CurrentDateTime>();
                services.AddTransient<IContractFeedProcessorHttpClient, ContractFeedProcessorHttpClient>();
                services.AddTransient<IContractFeedEventValidator, ContractFeedEventValidator>();
                services.AddTransient<IContractFeedReader, ContractFeedReader>();
                services.AddTransient<IContractDataProvider, ContractFeedProcessor>();
                services.AddTransient<IProviderAgreementStatusRepository, ProviderAgreementStatusRepository>(); //TODO
                services.AddTransient<IProviderAgreementStatusService, ProviderAgreementStatusService>();
                services.AddLogging();
            });

            return hostBuilder;
        }

        public static IHostBuilder AddConfiguration(this IHostBuilder hostBuilder)
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = ConfigurationManager.AppSettings["EnvironmentName"];
            }
            if (environment.Equals("LOCAL") || environment.Equals("AT") || environment.Equals("TEST"))
            {
                PopulateSystemDetails(environment);
            }

            return hostBuilder.ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json", true, true) // should this be conditional to Local and Dev envs?
                    .AddJsonFile($"appsettings.{environment}.json", true, true) // should this be conditional to Local and Dev envs?
                    .AddAzureTableStorage(options =>
                    {
                        options.ConfigurationKeys = ConfigurationManager.AppSettings["ConfigNames"].Split(",");
                        options.StorageConnectionString = ConfigurationManager.AppSettings["ConfigurationStorageConnectionString"];
                        options.EnvironmentName = environment;
                        options.PreFixConfigurationKeys = false;
                    })
                    .AddEnvironmentVariables();
            });
        }

        private static void PopulateSystemDetails(string envName)
        {
            SystemDetails.EnvironmentName = envName;
            SystemDetails.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
