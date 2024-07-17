using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings;

namespace SFA.DAS.PAS.Account.Application.Queries.GetUserNotificationSettings;

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
        
        _logger.LogInformation("Attempted to retrieve settings for user {UserRef} with email {Email}. UserSettings result: {Settings}", message.UserRef, message.Email, JsonSerializer.Serialize(userSettings));

        if (!userSettings.Any())
        {
            _logger.LogInformation("No settings found. Creating user settings for userRef {UserRef} using email {Email}", message.UserRef, message.Email);

            await _userRepository.AddSettings(message.Email);
            
            userSettings = (await _userRepository.GetUserSetting(message.UserRef, message.Email)).ToList();

            _logger.LogInformation("Created default settings for user {UserRef}. UserSettings result: {Settings}", message.UserRef, JsonSerializer.Serialize(userSettings));
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