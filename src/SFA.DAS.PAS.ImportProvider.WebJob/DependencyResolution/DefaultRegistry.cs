using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Providers.Api.Client;
using StructureMap;
using System;
using System.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
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

            var config = GetConfiguration("SFA.DAS.ProviderApprenticeshipsService");
            For<ProviderApprenticeshipsServiceConfiguration>().Use(config);
            For<IConfiguration>().Use<ProviderApprenticeshipsServiceConfiguration>();
            For<IProviderApiClient>().Use<ProviderApiClient>().Ctor<string>("baseUrl").Is(ctx => ctx.GetInstance<ProviderApprenticeshipsServiceConfiguration>().ApprenticeshipInfoService.BaseUrl);

            For<ILog>().Use(x => new NLogLogger(
               x.ParentType,
               new DummyRequestContext(),
               null)).AlwaysUnique();
        }

        private ProviderApprenticeshipsServiceConfiguration GetConfiguration(string serviceName)
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = ConfigurationManager.AppSettings["EnvironmentName"];
            }

            var configurationRepository = GetConfigurationRepository();
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(serviceName, environment, "1.0"));

            var result = configurationService.Get<ProviderApprenticeshipsServiceConfiguration>();

            return result;
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            return new AzureTableStorageConfigurationRepository(ConfigurationManager.AppSettings["ConfigurationStorageConnectionString"]);
        }
    }

    public class DummyRequestContext : ILoggingContext
    {
        public string Url { get; }

        public string IpAddress { get; }
    }
}
