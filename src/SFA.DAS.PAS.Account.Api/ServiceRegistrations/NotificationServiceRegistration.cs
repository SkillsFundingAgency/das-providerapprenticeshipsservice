using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Http;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.Notifications.Api.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace SFA.DAS.PAS.Account.Api.ServiceRegistrations
{
    public static class NotificationServiceRegistration
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
}
