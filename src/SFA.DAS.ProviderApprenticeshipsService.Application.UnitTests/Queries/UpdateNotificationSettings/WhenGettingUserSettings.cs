﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Settings;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetUserNotificationSettings
{
    [TestFixture]
    public class WhenGettingUserSettings
    {
        private GetUserNotificationSettingsHandler _sut;
        private Mock<IUserSettingsRepository> _mockSettingsRepo;
        const string UserRef = "userRef";
        [SetUp]
        public void SetUp()
        {
            _mockSettingsRepo = new Mock<IUserSettingsRepository>();
            _sut = new GetUserNotificationSettingsHandler(_mockSettingsRepo.Object, Mock.Of<IProviderCommitmentsLogger>());
        }

        [Test]
        public async Task ThenGettingUsersSettings()
        {
            _mockSettingsRepo.Setup(m => m.GetUserSetting(UserRef))
                .ReturnsAsync(new List<UserSetting> {new UserSetting { ReceiveNotifications = true, UserId = 1, UserRef = UserRef} });

            var result = await _sut.Handle(new GetUserNotificationSettingsQuery { UserRef =  UserRef}, new CancellationToken());

            result.NotificationSettings.Count.Should().Be(1);

            _mockSettingsRepo.Verify(m => m.GetUserSetting(It.IsAny<string>()), Times.Once);
            result.NotificationSettings.First().ShouldBeEquivalentTo(new UserNotificationSetting { UserRef = UserRef, ReceiveNotifications = true });
        }

        [Test]
        public async Task ThenCreatingSettingsIfNoSettingsFoundGettingUsersSettings()
        {
            var result = await _sut.Handle(new GetUserNotificationSettingsQuery { UserRef = UserRef }, new CancellationToken());

            result.NotificationSettings.Count.Should().Be(0);

            _mockSettingsRepo.Verify(m => m.AddSettings(It.IsAny<string>()), Times.Exactly(1));
            _mockSettingsRepo.Verify(m => m.GetUserSetting(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
