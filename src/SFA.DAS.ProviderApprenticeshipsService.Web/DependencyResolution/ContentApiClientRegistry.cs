using System.Net.Http;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using StructureMap;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.DependencyResolution
{
    public class ContentApiClientRegistry : Registry
    {
        public ContentApiClientRegistry()
        {
            For<ContentClientApiConfiguration>().Use(c => c.GetInstance<ProviderApprenticeshipsServiceConfiguration>().ContentApi);
            For<IContentApiConfiguration>().Use(c => c.GetInstance<ContentClientApiConfiguration>());
            For<IContentApiClient>().Use<ContentApiClient>().Ctor<HttpClient>().Is(c => CreateClient(c));
        }

        private HttpClient CreateClient(IContext context)
        {
            var config = context.GetInstance<IContentApiConfiguration>();

            HttpClient httpClient = new HttpClientBuilder()
                    .WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config))
                    .WithHandler(new RequestIdMessageRequestHandler())
                    .WithHandler(new SessionIdMessageRequestHandler())
                    .WithDefaultHeaders()
                    .Build();


            return httpClient;
        }
    }
}