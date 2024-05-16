﻿using System;
using System.Net.Http;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.DfESignIn.Auth.Api.Client;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.Http;
using SFA.DAS.Http.Configuration;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Configuration;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
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
                .AddAzureTableStorage(new []{ConfigurationKeys.ProviderApprenticeshipsService,ConfigurationKeys.DfESignInService})
                .AddEnvironmentVariables();
        });
    }

    public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices((context, services) =>
        {
            
            services.Configure<ProviderApprenticeshipsServiceConfiguration>(c =>
                context.Configuration.GetSection(ConfigurationKeys.ProviderApprenticeshipsService).Bind(c));
            
            services.Configure<DfEOidcConfiguration>(context.Configuration.GetSection($"{ConfigurationKeys.DfESignInService}:DfEOidcConfiguration"));
            services.Configure<DfEOidcConfiguration>(context.Configuration.GetSection($"{ConfigurationKeys.DfESignInService}:DfEOidcConfiguration_ProviderRoATP"));
            
            services.AddSingleton(cfg => cfg.GetService<IOptions<DfEOidcConfiguration>>().Value);
            services.AddSingleton<IBaseConfiguration>(isp =>
                isp.GetService<IOptions<ProviderApprenticeshipsServiceConfiguration>>().Value);
            services.AddSingleton(isp =>
                isp.GetService<IOptions<ProviderApprenticeshipsServiceConfiguration>>().Value.CommitmentNotification);

            services.AddHttpClient<IApiHelper, DfeSignInApiHelper>
                (
                    options => options.Timeout = TimeSpan.FromMinutes(30)
                )
                .SetHandlerLifetime(TimeSpan.FromMinutes(10))
                .AddPolicyHandler(HttpClientRetryPolicy());
            services.AddTransient<ITokenDataSerializer, TokenDataSerializer>();
            services.AddTransient<ITokenBuilder, TokenBuilder>();
            
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
    private static IAsyncPolicy<HttpResponseMessage> HttpClientRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                retryAttempt)));
    }
}