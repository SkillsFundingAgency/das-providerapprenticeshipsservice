using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Http;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.Notifications.Api.Client;
using System.Net.Http;
using SFA.DAS.Http.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using StructureMap;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.Commitments.Api.Client.Configuration;
using Microsoft.Extensions.Options;
using SFA.DAS.ProviderApprenticeshipsService.Web.DependencyResolution;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using SFA.DAS.ProviderApprenticeshipsService.Web.App_Start;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations
{
    public static class CommitmentsServiceRegistrations
    {
        public static IServiceCollection AddCommitments(this IServiceCollection services, IConfiguration configuration)
        {
            // IPROVIDERCOMMITMENTS

            // this is the old CommitmentsAPI, need to see if this can be switched to v2 (although it seems V2 doesnt provide
            // an equivalent method for GetProviders() in V1
            // ALSO, to verify it this resolves having added SFA.DAS.CommitmentsAPI to ConfigNames
            services.AddSingleton<ICommitmentsApiClientConfiguration>(configuration.Get<CommitmentsApiClientConfiguration>());
            //services.AddSingleton<ICommitmentsApiClientConfiguration>(cfg => cfg.GetService<IOptions<CommitmentsApiClientConfiguration>>().Value);

            services.AddTransient<IProviderCommitmentsApi>(s =>
            {
                var config = s.GetService<CommitmentsApiClientConfiguration>();
                var httpClient = GetHttpClient(config);
                return new ProviderCommitmentsApi(httpClient, config);
            });

            // ICOMMITMENTSV2APICLIENT
            services.Configure<CommitmentsApiClientV2Configuration>(configuration.GetSection("CommitmentsApiClientV2"));
            services.AddSingleton(cfg => cfg.GetService<IOptions<CommitmentsApiClientV2Configuration>>().Value);

            services.AddSingleton<ICommitmentsV2ApiClient, CommitmentsV2ApiClient>();
            services.AddHttpClient<ICommitmentsV2ApiClient, CommitmentsV2ApiClient>();
            //.AddHttpMessageHandler<RequestIdMessageRequestHandler>()
            //.AddHttpMessageHandler<SessionIdMessageRequestHandler>();

            /*
           services.AddSingleton<ICommitmentsV2ApiClient>(s =>
           {
               var commitmentsV2Config = s.GetService<CommitmentsApiClientV2Configuration>();
               var httpClient = GetHttpV2Client(commitmentsV2Config, configuration);
               return new CommitmentsV2ApiClient(httpClient, commitmentsV2Config, new ILogger<CommitmentsV2ApiClient>());
           });*/

            return services;
        }

        private static HttpClient GetHttpClient(ICommitmentsApiClientConfiguration config)
        {
            var httpClientBuilder = string.IsNullOrWhiteSpace(config.ClientId)
                ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config))
                : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config));

            return httpClientBuilder
                .WithDefaultHeaders()
                //.WithHandler(new RequestIdMessageRequestHandler())
                //.WithHandler(new SessionIdMessageRequestHandler())
                .Build();
        }

        private static HttpClient GetHttpV2Client(CommitmentsApiClientV2Configuration commitmentsV2Config, IConfiguration config)
        {
            var httpClientBuilder = !config.IsDev()
                ? new HttpClientBuilder()
                : new HttpClientBuilder().WithBearerAuthorisationHeader(new ManagedIdentityTokenGenerator(commitmentsV2Config));

            return httpClientBuilder
                .WithDefaultHeaders()
                //.WithHandler(new RequestIdMessageRequestHandler())
                //.WithHandler(new SessionIdMessageRequestHandler())
                .Build();
        }
    }

    // replacing:
    /*
     *  public CommitmentsRegistry()
        {
            For<CommitmentsApiClientConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<CommitmentsApiClientConfiguration>(ConfigurationKeys.CommitmentsApiClient)).Singleton();
            For<ICommitmentsApiClientConfiguration>().Use(c => c.GetInstance<CommitmentsApiClientConfiguration>());

            For<IProviderCommitmentsApi>().Use<ProviderCommitmentsApi>()
              .Ctor<HttpClient>().Is(c => GetHttpClient(c));

            For<CommitmentsApiClientV2Configuration>().Use(c => c.GetInstance<ProviderApprenticeshipsServiceConfiguration>().CommitmentsApiClientV2);

            For<ICommitmentsV2ApiClient>().Use<CommitmentsV2ApiClient>()
                .Ctor<HttpClient>().Is(c => GetHttpV2Client(c));
        }

        private HttpClient GetHttpClient(IContext context)
        {
            var config = context.GetInstance<CommitmentsApiClientConfiguration>();

            var httpClientBuilder = string.IsNullOrWhiteSpace(config.ClientId)
                ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config as IJwtClientConfiguration))
                : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config as IAzureActiveDirectoryClientConfiguration));

            return httpClientBuilder
                .WithDefaultHeaders()
                //.WithHandler(new RequestIdMessageRequestHandler())
                //.WithHandler(new SessionIdMessageRequestHandler())
                .Build();
        }

        private HttpClient GetHttpV2Client(IContext context)
        {
            var config = context.GetInstance<CommitmentsApiClientV2Configuration>();

            bool isDevelopment = ConfigurationManager.AppSettings["EnvironmentName"]?.Equals("LOCAL") ?? false;
            var httpClientBuilder = isDevelopment
                ? new HttpClientBuilder()
                : new HttpClientBuilder().WithBearerAuthorisationHeader(new ManagedIdentityTokenGenerator(config));

            return httpClientBuilder
                .WithDefaultHeaders()
                //.WithHandler(new RequestIdMessageRequestHandler())
                //.WithHandler(new SessionIdMessageRequestHandler())
                .Build();
        }
     */
}
