using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Providers.Api.Client;
using StructureMap;

namespace SFA.DAS.PAS.UpdateProvider.WebJob.DependencyResolution
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

            For<IProviderApiClient>().Use<ProviderApiClient>().Ctor<string>("baseUrl").Is(ctx => ctx.GetInstance<ApprenticeshipInfoServiceConfiguration>().BaseUrl);
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            return new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
        }
    }
}
