using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Http;
using SFA.DAS.PAS.ImportProvider.WebJob.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using System;
using System.Net.Http;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration;
using SFA.DAS.PAS.ImportProvider.WebJob.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.ImportProvider.WebJob.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((context, services) =>
            {
                services.Configure<ProviderApprenticeshipsServiceConfiguration>(context.Configuration.GetSection(ConfigurationKeys.ProviderApprenticeshipsService));
                services.AddSingleton(isp => isp.GetService<IOptions<ProviderApprenticeshipsServiceConfiguration>>().Value);

                services.Configure<CommitmentsApiClientV2Configuration>(context.Configuration.GetSection(ConfigurationKeys.ProviderApprenticeshipsService).GetSection("CommitmentsApiClientV2"));
                services.AddSingleton(cfg => cfg.GetService<IOptions<CommitmentsApiClientV2Configuration>>().Value);
                services.AddSingleton<ICommitmentsV2ApiClient>(s =>
                {
                    ILogger<CommitmentsV2ApiClient> commitmentsV2ApiLogger = new LoggerFactory().CreateLogger<CommitmentsV2ApiClient>();
                    var commitmentsV2Config = s.GetService<CommitmentsApiClientV2Configuration>();
                    var httpClient = GetHttpV2Client(commitmentsV2Config, context.Configuration);

                    return new CommitmentsV2ApiClient(httpClient, commitmentsV2Config, commitmentsV2ApiLogger);
                });

                services.AddTransient<IProviderRepository, ProviderRepository>();
                services.AddTransient<IImportProviderService, ImportProviderService>();
                services.AddLogging();
            });

            return hostBuilder;
        }

        public static IHostBuilder AddConfiguration(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureAppConfiguration((context, builder) =>
            {
                var environment = context.HostingEnvironment.EnvironmentName;

                builder.AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{environment}.json", true, true)
                    .AddAzureTableStorage(ConfigurationKeys.ProviderApprenticeshipsService)
                    .AddEnvironmentVariables();
            });
        }

        public static IHostBuilder UseDasEnvironment(this IHostBuilder hostBuilder)
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = Environment.GetEnvironmentVariable(EnvironmentVariableNames.EnvironmentName);
            }

            return hostBuilder.UseEnvironment(environment);
        }

        private static HttpClient GetHttpV2Client(CommitmentsApiClientV2Configuration commitmentsV2Config, IConfiguration config)
        {
            var httpClientBuilder = config.IsLocal()
                ? new HttpClientBuilder()
                : new HttpClientBuilder().WithBearerAuthorisationHeader(new ManagedIdentityTokenGenerator(commitmentsV2Config));
            
            return httpClientBuilder
                .WithDefaultHeaders()
                //.WithHandler(new RequestIdMessageRequestHandler())
                //.WithHandler(new SessionIdMessageRequestHandler())
                .Build();
        }
    }
}
