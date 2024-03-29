﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

public class BackgroundNotificationService : IBackgroundNotificationService
{
    private readonly ILogger<BackgroundNotificationService> _logger;
    private readonly INotificationsApi _notificationsApi;

    public BackgroundNotificationService(ILogger<BackgroundNotificationService> logger, INotificationsApi notificationsApi)
    {
        _logger = logger;
        _notificationsApi = notificationsApi;
    }

    public async Task SendEmail(Email email)
    {
        _logger.LogDebug("Sending email with ID: [{SystemId}] in a background task.", email.SystemId);

        try
        {
            await _notificationsApi.SendEmail(email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error using the Notification Api when trying to send email with ID: [{SystemId}].", email.SystemId);
        }
    }
}