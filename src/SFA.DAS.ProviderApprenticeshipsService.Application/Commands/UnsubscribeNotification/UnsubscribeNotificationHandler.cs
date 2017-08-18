using System.Collections.Generic;
using System.Threading.Tasks;

using FluentValidation.Results;

using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UnsubscribeNotification
{
    public class UnsubscribeNotificationHandker : AsyncRequestHandler<UnsubscribeNotificationRequest>
    {
        private readonly IUserSettingsRepository _settingsRepository;

        public UnsubscribeNotificationHandker(IUserSettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        protected override async Task HandleCore(UnsubscribeNotificationRequest request)
        {
            if (string.IsNullOrEmpty(request.UserRef))
                throw new InvalidRequestException(new List<ValidationFailure>(1) { new ValidationFailure("UserRef", "UserRef cannot be null or empty")} );

            await _settingsRepository.UpdateUserSettings(request.UserRef, false);
        }
    }
}
