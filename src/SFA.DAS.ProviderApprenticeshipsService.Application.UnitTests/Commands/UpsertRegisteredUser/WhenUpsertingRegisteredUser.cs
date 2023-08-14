using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services.UserIdentityService;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.UpsertRegisteredUser;

[TestFixture]
public class WhenUpsertingRegisteredUser
{
    private IRequestHandler<UpsertRegisteredUserCommand> _handler;
    private Mock<IValidator<UpsertRegisteredUserCommand>> _validator;
    private Mock<IUserIdentityService> _userIdentityService;

    [SetUp]
    public void Arrange()
    {
        _validator = new Mock<IValidator<UpsertRegisteredUserCommand>>();
        _validator.Setup(x => x.Validate(It.IsAny<UpsertRegisteredUserCommand>()))
            .Returns(new ValidationResult());

        _userIdentityService = new Mock<IUserIdentityService>();
        _userIdentityService.Setup(x => x.UpsertUserIdentityAttributes(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(() => Task.FromResult(new Unit()));

        _handler = new UpsertRegisteredUserCommandHandler(_validator.Object, _userIdentityService.Object);
    }

    [Test]
    public void ThenAnExceptionIsThrownIfTheCommandIsInvalid()
    {
        //Arrange
        _validator.Setup(x => x.Validate(It.IsAny<UpsertRegisteredUserCommand>()))
            .Returns(new ValidationResult(
                new List<ValidationFailure>
                {
                    new("TEST", "ERROR")
                }
            ));

        var command = new UpsertRegisteredUserCommand();

        //Act & Assert
        Assert.ThrowsAsync<ValidationException>(() =>_handler.Handle(command, new CancellationToken()));
    }

    [Test]
    public async Task ThenTheUserIdentityServiceIsCalledToUpsertRegisteredUser()
    {
        //Arrange
        var command = new UpsertRegisteredUserCommand
        {
            UserRef = "UserRef",
            DisplayName = "DisplayName",
            Email = "Email",
            Ukprn = 12345
        };

        //Act
        await _handler.Handle(command, new CancellationToken());

        //Assert
        _userIdentityService.Verify(x => x.UpsertUserIdentityAttributes(command.UserRef, 
                command.Ukprn,
                command.DisplayName,
                command.Email),
            Times.Once);
    }
}