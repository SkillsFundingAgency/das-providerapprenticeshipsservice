using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification
{
    public sealed class SendNotificationCommandHandler : AsyncRequestHandler<SendNotificationCommand>
    {
        private readonly IValidator<SendNotificationCommand> _validator;
        private readonly IBackgroundNotificationService _backgroundNotificationService;
        private readonly ILog _logger;

        public SendNotificationCommandHandler(IValidator<SendNotificationCommand>validator, IBackgroundNotificationService backgroundNotificationService, ILog logger)
        {
            _validator = validator;
            _backgroundNotificationService = backgroundNotificationService;
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
                await _backgroundNotificationService.SendEmail(message.Email);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Error calling Notification Api. Recipient: {message.Email.RecipientsAddress}");
            }
        }
    }
}
