using System;
using System.Configuration;
using System.Linq;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration
{
    public class ConfigurationPolicy<T> : ConfiguredInstancePolicy where T : Domain.Interfaces.IConfiguration
    {
        private readonly string _serviceName;

        public ConfigurationPolicy(string serviceName)
        {
            _serviceName = serviceName;
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var serviceConfigurationParamater = instance?.Constructor?.GetParameters().FirstOrDefault(x => x.ParameterType == typeof(T)
                                                            || ((System.Reflection.TypeInfo)typeof(T)).GetInterface(x.ParameterType.Name) != null);
           
            if (serviceConfigurationParamater != null)
            {
                var environment = Environment.GetEnvironmentVariable("DASENV");
                if (string.IsNullOrEmpty(environment))
                {
                    environment = ConfigurationManager.AppSettings["EnvironmentName"];
                }

                var configurationRepository = GetConfigurationRepository();
                var configurationService = new ConfigurationService(configurationRepository,
                    new ConfigurationOptions(_serviceName, environment, "1.0"));

                var result = configurationService.Get<T>();
                if (result != null)
                {
                    instance.Dependencies.AddForConstructorParameter(serviceConfigurationParamater, result);
                }
            }
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            IConfigurationRepository configurationRepository;
            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository = new AzureTableStorageConfigurationRepository(ConfigurationManager.AppSettings["ConfigurationStorageConnectionString"]);
            }
            return configurationRepository;
        }

    }
}