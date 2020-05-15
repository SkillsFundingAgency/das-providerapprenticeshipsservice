using System;
using System.Net.Http;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using StructureMap;
using IConfiguration = SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.IConfiguration;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.ProviderApprenticeshipsService";

        public DefaultRegistry()
        {
            Scan(
               scan =>
               {
                   scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                   scan.RegisterConcreteTypesAgainstTheFirstInterface();
               });

            var config = GetConfiguration();
            For<IConfiguration>().Use<ProviderApprenticeshipsServiceConfiguration>();
            ConfigureHttpClient(config);
            RegisterExecutionPolicies();

            For<ILog>().Use(x => new NLogLogger(
               x.ParentType,
               new DummyRequestContext(),
               null)).AlwaysUnique();
        }

        private ProviderApprenticeshipsServiceConfiguration GetConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            var configurationRepository = GetConfigurationRepository();
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(ServiceName, environment, "1.0"));

            var result = configurationService.Get<ProviderApprenticeshipsServiceConfiguration>();

            return result;
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            return new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
        }

        private void ConfigureHttpClient(ProviderApprenticeshipsServiceConfiguration config)
        {
            For<ProviderApprenticeshipsService.Infrastructure.Data.IHttpClientWrapper>()
                .Use<ProviderApprenticeshipsService.Infrastructure.Data.HttpClientWrapper>()
                .Ctor<HttpClient>()
                .Is(c => GetHttpClient(c));
        }

        private HttpClient GetHttpClient(IContext context)
        {
            var config = context.GetInstance<ProviderApprenticeshipsServiceConfiguration>();

            return new HttpClientBuilder()
                .WithDefaultHeaders()
                .WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config.CommitmentNotification))
                .Build();
        }

        private void RegisterExecutionPolicies()
        {
            For<ProviderApprenticeshipsService.Infrastructure.ExecutionPolicies.ExecutionPolicy>()
                .Use<ProviderApprenticeshipsService.Infrastructure.ExecutionPolicies.IdamsExecutionPolicy>()
                .Named(ProviderApprenticeshipsService.Infrastructure.ExecutionPolicies.IdamsExecutionPolicy.Name);
        }
    }

    public class DummyRequestContext : ILoggingContext
    {
        public string Url { get; }

        public string IpAddress { get; }
    }
}
