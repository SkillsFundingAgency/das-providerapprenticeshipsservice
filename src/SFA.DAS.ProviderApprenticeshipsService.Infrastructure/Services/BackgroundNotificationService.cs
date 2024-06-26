using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Notifications.Messages.Commands;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

public interface IBackgroundNotificationService
{
    Task SendEmail(NotificationEmail email);
}

public record NotificationEmail(string TemplateId, string RecipientsAddress, IReadOnlyDictionary<string, string> Tokens);

public class BackgroundNotificationService : IBackgroundNotificationService
{
    private readonly ILogger<BackgroundNotificationService> _logger;
    private readonly IMessageSession _publisher;

    public BackgroundNotificationService(ILogger<BackgroundNotificationService> logger, IMessageSession publisher)
    {
        _logger = logger;
        _publisher = publisher;
    }

    public async Task SendEmail(NotificationEmail email)
    {
        _logger.LogInformation("Sending email with TemplateID: [{TemplateId}] in a background task.", email.TemplateId);

        var command = new SendEmailCommand(email.TemplateId, email.RecipientsAddress, email.Tokens);

        try
        {
            await _publisher.Send(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error using the Notification Api when trying to send email with TemplateId: [{TemplateId}].", email.TemplateId);
        }
    }
}