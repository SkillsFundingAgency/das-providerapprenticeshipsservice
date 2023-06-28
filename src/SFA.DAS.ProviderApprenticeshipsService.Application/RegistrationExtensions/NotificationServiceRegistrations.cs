using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;

public static class NotificationServiceRegistrations
{
    public static IServiceCollection AddNotifications(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<NotificationsApiClientConfiguration>(c => configuration.GetSection("NotificationApi").Bind(c));
        services.AddSingleton(cfg => cfg.GetService<IOptions<NotificationsApiClientConfiguration>>().Value);

        services.AddTransient<INotificationsApi>(s =>
        {
            var config = s.GetService<NotificationsApiClientConfiguration>();
            var httpClient = GetHttpClient(config);
            return new NotificationsApi(httpClient, config);
        });

        services.AddTransient<IBackgroundNotificationService, BackgroundNotificationService>();

        return services;
    }

    private static HttpClient GetHttpClient(INotificationsApiClientConfiguration config)
    {
        var httpClient = string.IsNullOrWhiteSpace(config.ClientId)
            ? new HttpClientBuilder().WithBearerAuthorisationHeader(new JwtBearerTokenGenerator(config)).Build()
            : new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config)).Build();

        return httpClient;
    }
}