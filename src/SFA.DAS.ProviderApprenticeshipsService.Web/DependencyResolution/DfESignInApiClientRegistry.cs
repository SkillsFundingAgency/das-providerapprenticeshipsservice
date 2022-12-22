using SFA.DAS.AutoConfiguration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Api.Helpers;
using SFA.DAS.ProviderApprenticeshipsService.Web.App_Start;
using StructureMap;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.DependencyResolution
{
    /// <summary>
    /// DfESignInApiClientRegistry class to register the dependencies with IOC container.
    /// </summary>
    public class DfESignInApiClientRegistry : Registry
    {
        public DfESignInApiClientRegistry()
        {
            For<DfESignInServiceConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<DfESignInServiceConfiguration>(ConfigurationKeys.DfESignInConfiguration)).Singleton();
            For<IDfESignInServiceConfiguration>().Use(c => c.GetInstance<DfESignInServiceConfiguration>());
            For<IDfESignInService>().Use<DfESignInService>().Ctor<HttpClient>().Is(c => CreateClient());
            For<ITokenDataSerializer>().Use<TokenDataSerializer>();
            For<ITokenBuilder>().Use<TokenBuilder>();
        }

        /// <summary>
        /// Method to create the http client and inject it from constructor.
        /// </summary>
        /// <returns>HttpClient.</returns>
        private static HttpClient CreateClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken());
            return httpClient;
        }

        /// <summary>
        /// Method to generate the access token for DfESignIn Service.
        /// </summary>
        /// <returns>string.</returns>
        private static string AccessToken()
        {
            var tokenBuilder = StructuremapMvc.StructureMapDependencyScope.Container.GetInstance<ITokenBuilder>();
            if (tokenBuilder != null) return tokenBuilder.CreateToken();
            throw new NullReferenceException($"{nameof(tokenBuilder)} could not be null");
        }
    }
}