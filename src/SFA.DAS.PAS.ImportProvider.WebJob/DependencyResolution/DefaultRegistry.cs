using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.NLog.Logger;
using StructureMap;
using System;
using System.Configuration;
using System.Net.Http;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Http;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
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
            For<IProviderCommitmentsApi>().Use<ProviderCommitmentsApi>()
                .Ctor<HttpClient>().Is(c => GetHttpClient(c));
            
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
        
        private HttpClient GetHttpClient(IContext context)
        {
            var config = context.GetInstance<ProviderApprenticeshipsServiceConfiguration>();

            var httpClientBuilder = string.IsNullOrWhiteSpace(config.CommitmentsApi.ClientId)
                ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config as IJwtClientConfiguration))
                : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config as IAzureActiveDirectoryClientConfiguration));

            return httpClientBuilder
                .WithDefaultHeaders()
                .WithHandler(new RequestIdMessageRequestHandler())
                .WithHandler(new SessionIdMessageRequestHandler())
                .Build();
        }
    }

    public class DummyRequestContext : ILoggingContext
    {
        public string Url { get; }

        public string IpAddress { get; }
    }
}
