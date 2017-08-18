using System.Threading.Tasks;

using FluentValidation;
using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateUserNotificationSettings
{
    public class UpdateUserNotificationSettingsHandler : AsyncRequestHandler<UpdateUserNotificationSettingsCommand>
    {
        private readonly IUserSettingsRepository _userSettingsRepository;

        private readonly AbstractValidator<UpdateUserNotificationSettingsCommand> _validator;

        private readonly IProviderCommitmentsLogger _logger;

        public UpdateUserNotificationSettingsHandler(
            IUserSettingsRepository userSettingsRepository, 
            AbstractValidator<UpdateUserNotificationSettingsCommand> validator,
            IProviderCommitmentsLogger logger)
        {
            _userSettingsRepository = userSettingsRepository;
            _validator = validator;
            _logger = logger;
        }

        protected override async Task HandleCore(UpdateUserNotificationSettingsCommand command)
        {
            var validationResult = _validator.Validate(command);
            if (!validationResult.IsValid)
            {
                throw new InvalidRequestException(validationResult.Errors);
            }

            await _userSettingsRepository.UpdateUserSettings(command.UserRef, command.ReceiveNotifications);

            _logger.Trace($"User settings updated for user {command.UserRef}");
        }
    }
}
