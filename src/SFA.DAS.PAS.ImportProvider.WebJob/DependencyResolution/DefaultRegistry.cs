using SFA.DAS.NLog.Logger;
using StructureMap;
using System;
using System.Net.Http;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Http;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

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

            For<CommitmentsApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<CommitmentsApiClientConfiguration>("SFA.DAS.CommitmentsAPI")).Singleton();
            For<ICommitmentsApiClientConfiguration>().Use(c => c.GetInstance<CommitmentsApiClientConfiguration>());
            
            For<IProviderCommitmentsApi>().Use<ProviderCommitmentsApi>()
                .Ctor<HttpClient>().Is(c => GetHttpClient(c));
            
            For<ILog>().Use(x => new NLogLogger(
               x.ParentType,
               new DummyRequestContext(),
               null)).AlwaysUnique();
        }
        
        private HttpClient GetHttpClient(IContext context)
        {
            var config = context.GetInstance<CommitmentsApiClientConfiguration>();

            var httpClientBuilder = string.IsNullOrWhiteSpace(config.ClientId)
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
