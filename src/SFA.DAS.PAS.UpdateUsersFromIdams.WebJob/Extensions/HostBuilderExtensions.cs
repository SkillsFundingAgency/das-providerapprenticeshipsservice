using System;
using System.Net.Http;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Http;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Configuration;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Configurations;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Extensions;

public static class HostBuilderExtensions
{
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

    public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((context, services) =>
        {
            services.Configure<ProviderApprenticeshipsServiceConfiguration>(c =>
                context.Configuration.GetSection(ConfigurationKeys.ProviderApprenticeshipsService).Bind(c));
            services.AddSingleton<IBaseConfiguration>(isp =>
                isp.GetService<IOptions<ProviderApprenticeshipsServiceConfiguration>>().Value);
            services.AddSingleton(isp =>
                isp.GetService<IOptions<ProviderApprenticeshipsServiceConfiguration>>().Value.CommitmentNotification);

            services.AddTransient<IHttpClientWrapper>(s =>
            {
                var config = s.GetService<ProviderNotificationConfiguration>();
                var httpClient = GetHttpClient(config);
                return new HttpClientWrapper(httpClient);
            });

            services.AddSingleton(new ChainedTokenCredential(
                new ManagedIdentityCredential(),
                new AzureCliCredential())
            );

            services.AddTransient<IIdamsEmailServiceWrapper, IdamsEmailServiceWrapper>();
            services.AddTransient<IProviderRepository, ProviderRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IIdamsSyncService, IdamsSyncService>();

            services.AddHttpClient();

            services.AddLogging();
        });

        return hostBuilder;
    }

    public static IHostBuilder UseDasEnvironment(this IHostBuilder hostBuilder)
    {
        var environment = Environment.GetEnvironmentVariable("DASENV");
        if (string.IsNullOrEmpty(environment))
            environment = Environment.GetEnvironmentVariable(EnvironmentVariableNames.EnvironmentName);

        return hostBuilder.UseEnvironment(environment);
    }

    private static HttpClient GetHttpClient(IJwtClientConfiguration config)
    {
        return new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config)).Build();
    }
    
    public static IHostBuilder ConfigureDasLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureLogging((context, loggingBuilder) =>
        {
            var connectionString = context.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
            if (!string.IsNullOrEmpty(connectionString))
            {
                loggingBuilder.AddApplicationInsightsWebJobs(o => o.ConnectionString = connectionString);
            }

            loggingBuilder.AddConsole();
        });

        return hostBuilder;
    }
}