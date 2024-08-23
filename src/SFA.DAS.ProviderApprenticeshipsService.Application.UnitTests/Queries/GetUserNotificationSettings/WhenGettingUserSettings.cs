using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserSetting;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetUserNotificationSettings;

[TestFixture]
public class WhenGettingUserSettings
{
    private GetUserNotificationSettingsHandler _sut;
    private Mock<IUserSettingsRepository> _mockSettingsRepo;
    private Mock<ILogger<GetUserNotificationSettingsHandler>> _mockLogger;
    const string UserRef = "userRef";
    private const string Email = "email@test.com";

    [SetUp]
    public void SetUp()
    {
        _mockSettingsRepo = new Mock<IUserSettingsRepository>();
        _mockLogger = new Mock<ILogger<GetUserNotificationSettingsHandler>>();
        _sut = new GetUserNotificationSettingsHandler(_mockSettingsRepo.Object, _mockLogger.Object);
    }

    [Test]
    public async Task ThenGettingUsersSettingsByRefWhenNotNull()
    {
        _mockSettingsRepo.Setup(m => m.GetUserSetting(UserRef, Email))
            .ReturnsAsync(
                new List<UserSetting> { new() { ReceiveNotifications = true, UserId = 1, UserRef = UserRef } });

        var result = await _sut.Handle(new GetUserNotificationSettingsQuery { UserRef = UserRef, Email = Email},
            new CancellationToken());

        result.NotificationSettings.Count.Should().Be(1);

        _mockSettingsRepo.Verify(m => m.GetUserSetting(UserRef, Email), Times.Once);
        result.NotificationSettings.First().Should().BeEquivalentTo(new UserNotificationSetting
            { UserRef = UserRef, ReceiveNotifications = true, Email = Email});
    }
    
    [Test]
    public async Task ThenCreatingSettingsIfNoSettingsFoundGettingUsersSettings()
    {
        var result = await _sut.Handle(new GetUserNotificationSettingsQuery { UserRef = UserRef, Email = Email},
            new CancellationToken());

        result.NotificationSettings.Count.Should().Be(0);

        _mockSettingsRepo.Verify(m => m.AddSettings(Email), Times.Exactly(1));
        _mockSettingsRepo.Verify(m => m.GetUserSetting(UserRef, Email), Times.Exactly(2));
    }
}