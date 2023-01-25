using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NLog;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class BackgroundNotificationService : IBackgroundNotificationService
    {
        private readonly ILogger _logger;
        private readonly INotificationsApi _notificationsApi;

        public BackgroundNotificationService(ILogger logger, INotificationsApi notificationsApi)
        {
            _logger = logger;
            _notificationsApi = notificationsApi;
        }

        public Task SendEmail(Email email)
        {
            _logger.Debug($"Sending email with ID: [{email.SystemId}] in a background task.");

            try
            {
                _notificationsApi.SendEmail(email);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error using the Notification Api when trying to send email with ID: [{email.SystemId}].");
            }

            return Task.CompletedTask;
        }
    }
}