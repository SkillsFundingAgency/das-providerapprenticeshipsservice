using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings;

public class GetUserNotificationSettingsHandler : IRequestHandler<GetUserNotificationSettingsQuery, GetUserNotificationSettingsResponse>
{
    private readonly IUserSettingsRepository _userRepository;

    private readonly ILogger<GetUserNotificationSettingsHandler> _logger;

    public GetUserNotificationSettingsHandler(IUserSettingsRepository userRepository, ILogger<GetUserNotificationSettingsHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<GetUserNotificationSettingsResponse> Handle(GetUserNotificationSettingsQuery message, CancellationToken cancellationToken)
    {
        var userSettings = (await _userRepository.GetUserSetting(message.UserRef, message.Email)).ToList();

        if (!userSettings.Any())
        {
            _logger.LogInformation("No settings found for user {UserRef}", message.UserRef);
                
            await _userRepository.AddSettings(message.UserRef);
            userSettings = (await _userRepository.GetUserSetting(message.UserRef, message.Email)).ToList();

            _logger.LogInformation("Created default settings for user {UserRef}", message.UserRef);
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
                            Email = message.Email
                        }).ToList()
        };
    }
}