using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace SFA.DAS.PAS.Account.Application.Commands.SendNotification
{
    public sealed class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Unit>
    {
        private readonly IValidator<SendNotificationCommand> _validator;
        private readonly IBackgroundNotificationService _backgroundNotificationService;
        private readonly ILogger<SendNotificationCommandHandler> _logger;

        public SendNotificationCommandHandler(IValidator<SendNotificationCommand>validator, IBackgroundNotificationService backgroundNotificationService, ILogger<SendNotificationCommandHandler> logger)
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
                _logger.LogInformation("Invalid SendNotificationCommand, not sending");
                throw new ValidationException(validationResult.Errors);
            }
                

            _logger.LogInformation($"Sending email to {message.Email.RecipientsAddress}. Template: {message.Email.TemplateId}");

            try
            {
                await _backgroundNotificationService.SendEmail(message.Email);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error calling Notification Api. Recipient: {message.Email.RecipientsAddress}");
            }

            return Unit.Value;
        }
    }
}
