using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Http;
using SFA.DAS.PAS.ImportProvider.WebJob.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using System;
using System.Configuration;
using System.Net.Http;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.PAS.ImportProvider.WebJob.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                // this is the old CommitmentsAPI, need to see if this can be switched to v2 (although it seems V2 doesnt provide
                // an equivalent method for GetProviders() in V1
                // ALSO, to confirm where this config should come from???
                services.AddSingleton<ICommitmentsApiClientConfiguration>(context.Configuration.Get<CommitmentsApiClientConfiguration>());
                //services.AddSingleton<ICommitmentsApiClientConfiguration>(cfg => cfg.GetService<IOptions<CommitmentsApiClientConfiguration>>().Value);

                services.AddTransient<IProviderCommitmentsApi>(s =>
                {
                    var config = s.GetService<CommitmentsApiClientConfiguration>();
                    var httpClient = GetHttpClient(config);
                    return new ProviderCommitmentsApi(httpClient, config);
                });
                services.AddTransient<IProviderRepository, ProviderRepository>();
                services.AddTransient<IImportProviderService, ImportProviderService>();

            });

            return hostBuilder;
        }

        public static IHostBuilder AddConfiguration(this IHostBuilder hostBuilder)
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = ConfigurationManager.AppSettings["EnvironmentName"];
            }

            return hostBuilder.ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{environment}.json", true, true)
                    .AddAzureTableStorage(options =>
                    {
                        // NOTE: There is no section or standalone config relevant to this job currenty
                        // TBC: Where the config values are stored for CommitmentsApiClientConfiguration (presumably V1)
                        options.ConfigurationKeys = ConfigurationManager.AppSettings["ConfigNames"].Split(",");
                        options.StorageConnectionString = ConfigurationManager.AppSettings["ConfigurationStorageConnectionString"];
                        options.EnvironmentName = environment;
                        options.PreFixConfigurationKeys = false;
                    })
                    .AddEnvironmentVariables();
            });
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
    }
}
