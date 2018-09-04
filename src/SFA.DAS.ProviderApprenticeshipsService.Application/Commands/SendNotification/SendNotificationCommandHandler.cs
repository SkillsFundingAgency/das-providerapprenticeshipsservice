using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using MediatR;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification
{
    public sealed class SendNotificationCommandHandler : AsyncRequestHandler<SendNotificationCommand>
    {
        private readonly IValidator<SendNotificationCommand> _validator;
        private readonly INotificationsApi _notificationsApi;
        private readonly ILog _logger;

        public SendNotificationCommandHandler(IValidator<SendNotificationCommand>validator, INotificationsApi notificationsApi, ILog logger)
        {
            _validator = validator;
            _notificationsApi = notificationsApi;
            _logger = logger;
        }

        protected override async Task HandleCore(SendNotificationCommand message)
        {
            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid)
            {
                _logger.Info("Invalid SendNotificationCommand, not sending");
                throw new ValidationException(validationResult.Errors);
            }
                

            _logger.Info($"Sending email to {message.Email.RecipientsAddress}. Template: {message.Email.TemplateId}");

            try
            {
                await _notificationsApi.SendEmail(message.Email);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Error calling Notification Api. Recipient: {message.Email.RecipientsAddress}");
            }
        }
    }
}
