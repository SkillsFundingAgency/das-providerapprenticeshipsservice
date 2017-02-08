using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification
{
    public sealed class SendNotificationCommandHandler : AsyncRequestHandler<SendNotificationCommand>
    {
        private readonly AbstractValidator<SendNotificationCommand> _validator;
        private readonly INotificationsApi _notificationsApi;
        private readonly ILog _logger;

        public SendNotificationCommandHandler(AbstractValidator<SendNotificationCommand>validator, INotificationsApi notificationsApi, ILog logger)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (notificationsApi == null)
                throw new ArgumentNullException(nameof(notificationsApi));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _validator = validator;
            _notificationsApi = notificationsApi;
            _logger = logger;
        }

        protected override async Task HandleCore(SendNotificationCommand message)
        {
            _validator.ValidateAndThrow(message);

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
