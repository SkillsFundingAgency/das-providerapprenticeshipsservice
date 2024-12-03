using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Application.Exceptions;
using SFA.DAS.PAS.Account.Application.Queries.GetUser;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserProfile;

namespace SFA.DAS.PAS.Account.Application.UnitTests.Queries.GetUser;

[TestFixture]
public class WhenGettingUser
{
    private GetUserQueryHandler _sut;
    private Mock<ILogger<GetUserQueryHandler>> _mockLogger;
    private Mock<IUserRepository> _mockUserRepository;
    private const string UserRef = "userRef";

    [SetUp]
    public void SetUp()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<GetUserQueryHandler>>();
        _sut = new GetUserQueryHandler(_mockUserRepository.Object, _mockLogger.Object);
    }

    [Test]
    public void IfUserRefIsNullOrEmpty_InvalidRequestExceptionThrown()
    {
        // Arrange
        const string emptyUserRef = "";
        var query = new GetUserQuery { UserRef = emptyUserRef };

        var action = () => _sut.Handle(query, new CancellationToken());
        
        action.Should().ThrowAsync<InvalidRequestException>();
    }

    [Test]
    public async Task IfUserNotFound_EmptyResponseReturned()
    {
        // Arrange
        var query = new GetUserQuery { UserRef = UserRef };
        _mockUserRepository.Setup(m => m.GetUser(UserRef))
            .Returns(Task.FromResult<User>(null));

        // Act
        var result = await _sut.Handle(query, new CancellationToken());

        // Assert
        result.UserRef.Should().BeNull();
    }

    [Test]
    public async Task IfUserFound_UserResponseReturned()
    {
        // Arrange
        const string displayName = "testName";
        const string email = "testEmail";
        var query = new GetUserQuery { UserRef = UserRef };

        _mockUserRepository
            .Setup(m => m.GetUser(UserRef))
            .ReturnsAsync(new User
            {
                UserRef = UserRef,
                DisplayName = displayName,
                Email = email,
                Id = 1,
                IsDeleted = false,
                Ukprn = 143243,
            });

        // Act
        var result = await _sut.Handle(query, new CancellationToken());

        // Assert
        result.UserRef.Should().NotBeNull();
        result.UserRef.Should().Be(UserRef);
        result.Name.Should().Be(displayName);
        result.EmailAddress.Should().Be(email);
    }
}