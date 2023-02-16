using System;
using System.Reflection;

using System.Configuration;

using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.PAS.ContractAgreements.WebJob.Configuration;
using SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

using StructureMap;
using StructureMap.Graph;
using IConfiguration = SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.IBaseConfiguration;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.DependencyResolution
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
            For<IConfiguration>().Use(config);
            For<IProviderAgreementStatusConfiguration>().Use(config);
            For<ContractFeedConfiguration>().Use(config);
            For<ICurrentDateTime>().Use(x => new CurrentDateTime());
            For<IContractFeedProcessorHttpClient>().Use<ContractFeedProcessorHttpClient>();
            For<IContractDataProvider>().Use<ContractFeedProcessor>();
            For<IProviderAgreementStatusRepository>().Use<ProviderAgreementStatusRepository>();
            For<IContractFeedEventValidator>().Use<ContractFeedEventValidator>();
        }

        private ContractFeedConfiguration GetConfiguration(string serviceName)
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
            return new AzureTableStorageConfigurationRepository(ConfigurationManager.AppSettings["ConfigurationStorageConnectionString"]);
        }
    }
}