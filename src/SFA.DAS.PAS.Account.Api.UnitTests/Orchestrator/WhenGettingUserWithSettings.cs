using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Application.Queries.GetUser;
using SFA.DAS.PAS.Account.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings;

namespace SFA.DAS.PAS.Account.Api.UnitTests.Orchestrator;

[TestFixture]
public class WhenGettingUserWithSettings
{
    private IUserOrchestrator _sut;
    private Mock<IMediator> _mediator;
    private Mock<ILogger<UserOrchestrator>> _logger;

    [SetUp]
    public void SetUp()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<UserOrchestrator>>();
        _sut = new UserOrchestrator(_mediator.Object, _logger.Object);
    }

    [Test]
    public async Task ShouldReturnUserWithDetails()
    {
        // Arrange
        const string userRef = "ISP-3320242";
        const bool receiveNotifications = false;
        const string email = "test@email.com";
        const string name = "test";

        var notificationSettings = new UserNotificationSetting 
        {
            ReceiveNotifications = receiveNotifications
        };

        var getUserNotificationSettingsResponse = new GetUserNotificationSettingsResponse 
        {
            NotificationSettings = [notificationSettings]
        };

        var user = new GetUserQueryResponse
        {
            UserRef = userRef,
            EmailAddress = email,
            Name = name,
        };

        _mediator.Setup(m => m.Send(It.IsAny<GetUserNotificationSettingsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(getUserNotificationSettingsResponse);

        _mediator.Setup(m => m.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.GetUserWithSettings(userRef);

        // Assert
        result.UserRef.Should().Be(userRef);
        result.ReceiveNotifications.Should().Be(receiveNotifications);
        result.EmailAddress.Should().Be(email);
        result.DisplayName.Should().Be(name);
    }

    [Test]
    public async Task NoNotificationSettingsFoundForUser_ShouldReturnUserWithFalseReceiveNotifications()
    {
        // Arrange
        const string userRef = "ISP-3320242";
        const string email = "test@email.com";
        const string name = "test";

        var getUserNotificationSettingsResponse = new GetUserNotificationSettingsResponse
        {
            NotificationSettings = []
        };

        var user = new GetUserQueryResponse
        {
            UserRef = userRef,
            EmailAddress = email,
            Name = name,
        };

        _mediator.Setup(m => m.Send(It.IsAny<GetUserNotificationSettingsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(getUserNotificationSettingsResponse);

        _mediator.Setup(m => m.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.GetUserWithSettings(userRef);

        // Assert
        result.UserRef.Should().Be(userRef);
        result.ReceiveNotifications.Should().BeFalse();
        result.EmailAddress.Should().Be(email);
        result.DisplayName.Should().Be(name);
    }

    [Test]
    public async Task NoUserFound_ShouldReturnNull()
    {
        // Arrange
        const string userRef = "ISP-3320242";
        var getUserNotificationSettingsResponse = new GetUserNotificationSettingsResponse
        {
            NotificationSettings = []
        };
        var user = new GetUserQueryResponse { };

        _mediator.Setup(m => m.Send(It.IsAny<GetUserNotificationSettingsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(getUserNotificationSettingsResponse);

        _mediator.Setup(m => m.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _sut.GetUserWithSettings(userRef);

        // Assert
        result.Should().BeNull();
    }
}