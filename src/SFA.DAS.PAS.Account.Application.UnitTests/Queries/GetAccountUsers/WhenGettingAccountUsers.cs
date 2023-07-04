using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserProfile;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserSetting;

namespace SFA.DAS.PAS.Account.Application.UnitTests.Queries.GetAccountUsers;

[TestFixture]
public class WhenGettingAccountUsers
{
    private GetAccountUsersHandler _sut;
    private Mock<IUserSettingsRepository> _mockUserSettingsRepo;
    private Mock<IUserRepository> _mockUserRepo;
    private Mock<ILogger<GetAccountUsersHandler>> _mockLogger;
    private const string UserRef = "userRef";

    [SetUp]
    public void SetUp()
    {
        _mockUserSettingsRepo = new Mock<IUserSettingsRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<GetAccountUsersHandler>>();
        _sut = new GetAccountUsersHandler(_mockUserSettingsRepo.Object, _mockUserRepo.Object, _mockLogger.Object);
    }

    [Test]
    public Task IfUkprnIsNotSupplied_ValidationExcetionIsThrown()
    {
        // Arrange
        var ukprn = 0;
        var query = new GetAccountUsersQuery { Ukprn = ukprn };

        // Act and Assert
        Assert.That(async () => await _sut.Handle(query, new CancellationToken()), Throws.TypeOf<ValidationException>());
            
        return Task.CompletedTask;
    }

    [Test]
    public async Task IfUserNotFound_NoUserSettingsReturned()
    {
        // Arrange
        var ukprn = 10000528;
        var query = new GetAccountUsersQuery { Ukprn = ukprn };
        _mockUserRepo.Setup(m => m.GetUsers(ukprn))
            .ReturnsAsync(new List<User> { });

        // Act
        var result = await _sut.Handle(query, new CancellationToken());

        // Assert
        result.UserSettings.Count.Should().Be(0);
    }

    [Test]
    public async Task IfUserSettingsNotFound_NoUserSettingsReturned()
    {
        // Arrange
        var ukprn = 10000528;
        var query = new GetAccountUsersQuery { Ukprn = ukprn };

        _mockUserRepo.Setup(m => m.GetUsers(ukprn))
            .ReturnsAsync(new List<User> { new User
                {
                    Id = 1,
                    UserRef = UserRef,
                    DisplayName = "name",
                    Email = "email",
                    Ukprn = ukprn,
                    IsDeleted = false,
                    UserType = UserType.NormalUser
                }
            });

        _mockUserSettingsRepo.Setup(m => m.GetUserSetting(UserRef))
            .ReturnsAsync(new List<UserSetting> { });

        // Act
        var result = await _sut.Handle(query, new CancellationToken());

        // Assert
        result.UserSettings.Count.Should().Be(0);
    }

    [Test]
    public async Task IfUserAndUserSettingsFound_UserSettingsReturned()
    {
        // Arrange
        var ukprn = 10000528;
        var query = new GetAccountUsersQuery { Ukprn = ukprn };

        _mockUserRepo.Setup(m => m.GetUsers(ukprn))
            .ReturnsAsync(new List<User> { new User
                {
                    Id = 1,
                    UserRef = UserRef,
                    DisplayName = "name",
                    Email = "email",
                    Ukprn = ukprn,
                    IsDeleted = false,
                    UserType = UserType.NormalUser
                }
            });

        _mockUserSettingsRepo.Setup(m => m.GetUserSetting(UserRef))
            .ReturnsAsync(new List<UserSetting> { new UserSetting
                {
                    ReceiveNotifications = true,
                    UserId = 1,
                    UserRef = UserRef
                }
            });

        // Act
        var result = await _sut.Handle(query, new CancellationToken());

        // Assert
        result.UserSettings.Count.Should().Be(1);

        var userSetting = result.UserSettings.First();

        userSetting.Setting.UserRef.Should().Be(UserRef);
        userSetting.User.Id.Should().Be(1);
        userSetting.User.Email.Should().Be("email");
        userSetting.User.Ukprn.Should().Be(ukprn);
    }
}