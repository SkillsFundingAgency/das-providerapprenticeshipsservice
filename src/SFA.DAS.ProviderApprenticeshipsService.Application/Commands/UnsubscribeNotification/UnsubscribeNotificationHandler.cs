using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation.Results;

using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UnsubscribeNotification
{
    public class UnsubscribeNotificationHandler : IRequestHandler<UnsubscribeNotificationRequest, Unit>
    {
        private readonly IUserSettingsRepository _settingsRepository;

        public UnsubscribeNotificationHandler(IUserSettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public async Task<Unit> Handle(UnsubscribeNotificationRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.UserRef))
                throw new InvalidRequestException(new List<ValidationFailure>(1) { new ValidationFailure("UserRef", "UserRef cannot be null or empty")} );

            await _settingsRepository.UpdateUserSettings(request.UserRef, false);

            return Unit.Value;
        }
    }
}
