using System;
using System.Reflection;

using Microsoft.Azure;

using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

using StructureMap;
using StructureMap.Graph;


namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.DependencyResolution
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

            var config = GetConfiguration("SFA.DAS.ContractAgreements");
            For<ContractFeedConfiguration>().Use(config);
            For<IContractFeedProcessorHttpClient>().Use<ContractFeedProcessorHttpClient>();
            For<IContractDataProvider>().Use<ContractFeedProcessor>();
            For<IProviderAgreementStatusRepository>().Use<ProviderAgreementStatusRepository>();
            For<IContractFeedEventValidator>().Use<ContractFeedEventValidator>();

            // ToDo: Implement overload for NLogLogger without IRequestContext 
            For<ILog>().Use(x => new NLogLogger(
                x.ParentType,
                new DummyRequestContext())).AlwaysUnique();
        }

        private ContractFeedConfiguration GetConfiguration(string serviceName)
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }
            if (environment.Equals("LOCAL") || environment.Equals("AT") || environment.Equals("TEST"))
            {
                PopulateSystemDetails(environment);
            }

            var configurationRepository = GetConfigurationRepository();
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(serviceName, environment, "1.0"));

            var result = configurationService.Get<ContractFeedConfiguration>();

            return result;
        }

        private void PopulateSystemDetails(string envName)
        {
            SystemDetails.EnvironmentName = envName;
            SystemDetails.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            return new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
        }
    }

    public class DummyRequestContext : IRequestContext
    {
        public string Url { get; }

        public string IpAddress { get; }
    }
}
