using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.NLog.Logger;
using SFA.DAS.PAS.ImportProvider.WebJob.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.Providers.Api.Client;
using StructureMap;
using System;

using IConfiguration = SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.IConfiguration;

namespace SFA.DAS.PAS.ImportProvider.WebJob.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
       public DefaultRegistry()
        {
            Scan(
               scan =>
               {
                   scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                   scan.RegisterConcreteTypesAgainstTheFirstInterface();
               });

            var config = GetConfiguration("SFA.DAS.ImportProviders");
            For<IConfiguration>().Use(config);
            For<ImportProviderConfiguration>().Use(config);
            For<IImportProviderConfiguration>().Use(config);
            For<IProviderApiClient>().Use<ProviderApiClient>().Ctor<string>("baseUrl").Is(ctx => ctx.GetInstance<ImportProviderConfiguration>().BaseUrl);

            For<ILog>().Use(x => new NLogLogger(
               x.ParentType,
               new DummyRequestContext(),
               null)).AlwaysUnique();
        }

        private ImportProviderConfiguration GetConfiguration(string serviceName)
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            var configurationRepository = GetConfigurationRepository();
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(serviceName, environment, "1.0"));

            var result = configurationService.Get<ImportProviderConfiguration>();

            return result;
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            return new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
        }
    }

    public class DummyRequestContext : ILoggingContext
    {
        public string Url { get; }

        public string IpAddress { get; }
    }
}
