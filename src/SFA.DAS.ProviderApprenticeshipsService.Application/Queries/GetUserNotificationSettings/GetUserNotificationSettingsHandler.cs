using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings
{
    public class GetUserNotificationSettingsHandler : IRequestHandler<GetUserNotificationSettingsQuery, GetUserNotificationSettingsResponse>
    {
        private readonly IUserSettingsRepository _userRepository;

        private readonly IProviderCommitmentsLogger _logger;

        public GetUserNotificationSettingsHandler(IUserSettingsRepository userRepository, IProviderCommitmentsLogger logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<GetUserNotificationSettingsResponse> Handle(GetUserNotificationSettingsQuery message, CancellationToken cancellationToken)
        {
            var userSettings = (await _userRepository.GetUserSetting(message.UserRef)).ToList();
            if (!userSettings.Any())
            {
                _logger.Info($"No settings found for user {message.UserRef}");
                await _userRepository.AddSettings(message.UserRef);
                userSettings = (await _userRepository.GetUserSetting(message.UserRef)).ToList();
                _logger.Info($"Created default settings for user {message.UserRef}");
            }

            return new GetUserNotificationSettingsResponse
                {
                    NotificationSettings =
                        userSettings.Select(
                            m =>
                            new UserNotificationSetting
                                {
                                    UserRef = m.UserRef,
                                    ReceiveNotifications =
                                        m.ReceiveNotifications,
                                }).ToList()
                };
        }
    }
}