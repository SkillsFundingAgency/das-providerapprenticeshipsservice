using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using NLog;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteRegisteredUser;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification
{
    public sealed class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Unit>
    {
        private readonly IValidator<SendNotificationCommand> _validator;
        private readonly IBackgroundNotificationService _backgroundNotificationService;
        private readonly ILogger _logger;

        public SendNotificationCommandHandler(IValidator<SendNotificationCommand>validator, IBackgroundNotificationService backgroundNotificationService, ILogger logger)
        {
            _validator = validator;
            _backgroundNotificationService = backgroundNotificationService;
            _logger = logger;
        }

        public async Task<Unit> Handle(SendNotificationCommand message, CancellationToken cancellationToken)
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
                _logger.Error(ex, $"Error calling Notification Api. Recipient: {message.Email.RecipientsAddress}");;
            }

            return Unit.Value;
        }
    }
}
