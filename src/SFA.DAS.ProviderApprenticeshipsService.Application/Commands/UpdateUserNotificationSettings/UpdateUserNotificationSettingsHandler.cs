﻿using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateUserNotificationSettings
{
    public class UpdateUserNotificationSettingsHandler : AsyncRequestHandler<UpdateUserNotificationSettingsCommand>
    {
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly IValidator<UpdateUserNotificationSettingsCommand> _validator;
        private readonly IProviderCommitmentsLogger _logger;

        public UpdateUserNotificationSettingsHandler(
            IUserSettingsRepository userSettingsRepository, 
            IValidator<UpdateUserNotificationSettingsCommand> validator,
            IProviderCommitmentsLogger logger)
        {
            _userSettingsRepository = userSettingsRepository;
            _validator = validator;
            _logger = logger;
        }

        protected override async Task Handle(UpdateUserNotificationSettingsCommand command, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(command);
            if (!validationResult.IsValid)
                throw new InvalidRequestException(validationResult.Errors);

            await _userSettingsRepository.UpdateUserSettings(command.UserRef, command.ReceiveNotifications);

            _logger.Trace($"User settings updated for user {command.UserRef}");
        }
    }
}
