using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Application.Queries.GetUserNotificationSettings;
using FluentAssertions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings;
using SFA.DAS.PAS.Account.Application.Queries.GetUser;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserSetting;

namespace SFA.DAS.PAS.Account.Api.UnitTests.Orchestrator
{
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
            var userRef = "ISP-3320242";
            var receiveNotifications = false;
            var superUser = false;
            var email = "test@email.com";
            var name = "test";

            var notificationSettings = new UserNotificationSetting 
            {
                ReceiveNotifications = receiveNotifications
            };

            var getUserNotificationSettingsResponse = new GetUserNotificationSettingsResponse 
            {
                NotificationSettings = new List<UserNotificationSetting>()
            };
            getUserNotificationSettingsResponse.NotificationSettings.Add(notificationSettings);

            var user = new GetUserResponse
            {
                UserRef = userRef,
                IsSuperUser = superUser,
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
            result.IsSuperUser.Should().Be(superUser);
            result.EmailAddress.Should().Be(email);
            result.DisplayName.Should().Be(name);
        }

        [Test]
        public async Task NoNotificationSettingsFoundForUser_ShouldReturnUserWithFalseReceiveNotifications()
        {
            // Arrange
            var userRef = "ISP-3320242";
            var superUser = false;
            var email = "test@email.com";
            var name = "test";

            var getUserNotificationSettingsResponse = new GetUserNotificationSettingsResponse
            {
                NotificationSettings = new List<UserNotificationSetting>()
            };

            var user = new GetUserResponse
            {
                UserRef = userRef,
                IsSuperUser = superUser,
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
            Assert.IsFalse(result.ReceiveNotifications);
            result.IsSuperUser.Should().Be(superUser);
            result.EmailAddress.Should().Be(email);
            result.DisplayName.Should().Be(name);
        }

        [Test]
        public async Task NoUserFound_ShouldReturnNull()
        {
            // Arrange
            var userRef = "ISP-3320242";
            var getUserNotificationSettingsResponse = new GetUserNotificationSettingsResponse
            {
                NotificationSettings = new List<UserNotificationSetting>()
            };
            var user = new GetUserResponse { };

            _mediator.Setup(m => m.Send(It.IsAny<GetUserNotificationSettingsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(getUserNotificationSettingsResponse);

            _mediator.Setup(m => m.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(user);

            // Act
            var result = await _sut.GetUserWithSettings(userRef);

            // Assert
            Assert.IsNull(result);
        }
    }
}
