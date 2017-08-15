using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings
{
    public class GetUserNotificationSettingsHandler : IAsyncRequestHandler<GetUserNotificationSettingsQuery, GetUserNotificationSettingsResponse>
    {
        private readonly IUserSettingsRepository _userRepository;

        public GetUserNotificationSettingsHandler(IUserSettingsRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetUserNotificationSettingsResponse> Handle(GetUserNotificationSettingsQuery message)
        {
            var users = await _userRepository.GetUserSetting(message.UserRef);

            return new GetUserNotificationSettingsResponse
                {
                    NotificationSettings =
                        users.Select(
                            m =>
                            new UserNotificationSetting
                                {
                                    AccountId = 1,
                                    HashedAccountId = "ABBA12",
                                    Id = 800,
                                    Name = m.UserRef,
                                    ReceiveNotifications =
                                        m.ReceiveNotifications,
                                    UserId = 999
                                }).ToList()
                };
        }
    }
}