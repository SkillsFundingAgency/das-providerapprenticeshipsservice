using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings;

namespace SFA.DAS.PAS.Account.Application.Queries.GetUserNotificationSettings;

public class GetUserNotificationSettingsHandler(IUserSettingsRepository userRepository, ILogger<GetUserNotificationSettingsHandler> logger)
    : IRequestHandler<GetUserNotificationSettingsQuery, GetUserNotificationSettingsResponse>
{
    public async Task<GetUserNotificationSettingsResponse> Handle(GetUserNotificationSettingsQuery message, CancellationToken cancellationToken)
    {
        var userSettings = (await userRepository.GetUserSetting(message.UserRef, message.Email)).ToList();
        
        logger.LogInformation("Attempted to retrieve settings for user {UserRef}", message.UserRef);

        if (!userSettings.Any())
        {
            logger.LogInformation("No settings found. Creating user settings for userRef {UserRef}", message.UserRef);

            await userRepository.AddSettings(message.Email);
            
            userSettings = (await userRepository.GetUserSetting(message.UserRef, message.Email)).ToList();

            logger.LogInformation("Created default settings for user {UserRef}", message.UserRef);
        }

        return new GetUserNotificationSettingsResponse
        {
            NotificationSettings = userSettings.Select(setting => new UserNotificationSetting
            {
                UserRef = setting.UserRef,
                ReceiveNotifications = setting.ReceiveNotifications,
            }).ToList()
        };
    }
}