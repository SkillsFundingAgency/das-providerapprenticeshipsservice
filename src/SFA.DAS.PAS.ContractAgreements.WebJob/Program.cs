// This application entry point is based on ASP.NET Core new project templates and is included
// as a starting point for app host configuration.
// This file may need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core hosting, see https://docs.microsoft.com/aspnet/core/fundamentals/host/web-host

using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.ContractAgreements.WebJob.Configuration;
using SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.PAS.ContractAgreements.WebJob.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.ContractAgreements.WebJob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger logger = loggerFactory.CreateLogger<Program>();

            try
            {
                var services = new ServiceCollection();
                // what about ContractFeedConfigration and loading config? Which config is it meant to come from?
                services.AddTransient<ICurrentDateTime, CurrentDateTime>();
                services.AddTransient<IContractFeedProcessorHttpClient, ContractFeedProcessorHttpClient>();
                services.AddTransient<IContractFeedEventValidator, ContractFeedEventValidator>();
                services.AddTransient<IContractFeedReader, ContractFeedReader>();                
                services.AddTransient<IContractDataProvider, ContractFeedProcessor>();
                services.AddTransient<IProviderAgreementStatusRepository, ProviderAgreementStatusRepository>();
                services.AddTransient<IProviderAgreementStatusService, ProviderAgreementStatusService>();
                services.AddLogging();
                var provider = services.BuildServiceProvider();

                var config = GetConfiguration("SFA.DAS.ContractAgreements");
                For<IConfiguration>().Use(config);
                For<IProviderAgreementStatusConfiguration>().Use(config);
                For<ContractFeedConfiguration>().Use(config);
                //For<ICurrentDateTime>().Use(x => new CurrentDateTime());
                //For<IContractFeedProcessorHttpClient>().Use<ContractFeedProcessorHttpClient>();
                //For<IContractDataProvider>().Use<ContractFeedProcessor>();
                For<IProviderAgreementStatusRepository>().Use<ProviderAgreementStatusRepository>();
                //For<IContractFeedEventValidator>().Use<ContractFeedEventValidator>();

                logger.LogInformation("ContractAgreements job started");

                var timer = Stopwatch.StartNew();

                var service = provider.GetService<ProviderAgreementStatusService>();
                service.UpdateProviderAgreementStatuses().Wait();

                timer.Stop();

                logger.LogInformation($"ContractAgreements job done, Took: {timer.ElapsedMilliseconds} milliseconds");
            }
            catch (AggregateException exc)
            {
                logger.LogError(exc, "Error running ContractAgreements WebJob");
                exc.Handle(ex =>
                {
                    logger.LogError(ex, "Inner exception running ContractAgreements WebJob");
                    return false;
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error running ContractAgreements WebJob");
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   var fasf = webBuilder.UseStartup<Startup>();
               });
    }
}
