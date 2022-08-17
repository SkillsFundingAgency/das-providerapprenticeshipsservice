using System;
using System.Configuration;
using System.Reflection;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using StructureMap;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.DependencyResolution
{
    public class RoatpCourseManagementWebRegistry : Registry
    {
        public RoatpCourseManagementWebRegistry()
        {
            var environment = GetAndStoreEnvironment();
            var configurationRepository = GetConfigurationRepository();
            For<IConfigurationRepository>().Use(configurationRepository);
           
            var config = GetConfiguration(environment, configurationRepository);
           
             For<IRoatpCourseManagementWebConfiguration>().Use(config);
        }

        private string GetAndStoreEnvironment()
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
        
            return environment;
        }

        private RoatpCourseManagementWebConfiguration GetConfiguration(string environment, IConfigurationRepository configurationRepository)
        {
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(RoatpConstants.ServiceName, environment, "1.0"));
        
            return configurationService.Get<RoatpCourseManagementWebConfiguration>();
        }
        
        private static IConfigurationRepository GetConfigurationRepository()
        {
            return new AzureTableStorageConfigurationRepository(ConfigurationManager.AppSettings["ConfigurationStorageConnectionString"]);
        }
        
        private void PopulateSystemDetails(string envName)
        {
            SystemDetails.EnvironmentName = envName;
            SystemDetails.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
