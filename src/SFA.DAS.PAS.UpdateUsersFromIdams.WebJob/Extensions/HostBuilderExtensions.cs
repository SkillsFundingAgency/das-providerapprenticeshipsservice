using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using System;
using SFA.DAS.Configuration;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Extensions
{
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
                services.Configure<ProviderApprenticeshipsServiceConfiguration>(context.Configuration.GetSection(ConfigurationKeys.ProviderApprenticeshipsService));
                services.AddSingleton<IBaseConfiguration>(isp => isp.GetService<IOptions<ProviderApprenticeshipsServiceConfiguration>>().Value);
                services.AddSingleton<IProviderNotificationConfiguration>(isp => isp.GetService<IOptions<ProviderApprenticeshipsServiceConfiguration>>().Value.CommitmentNotification);

                services.AddTransient<IHttpClientWrapper, HttpClientWrapper>();
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
            {
                environment = Environment.GetEnvironmentVariable(EnvironmentVariableNames.EnvironmentName);
            }

            return hostBuilder.UseEnvironment(environment);
        }
    }
}
