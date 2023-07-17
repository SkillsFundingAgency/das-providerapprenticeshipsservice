using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteRegisteredUser;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.DeleteRegisteredUser;

[TestFixture]
public class WhenDeletingRegisteredUser
{
    private IRequestHandler<DeleteRegisteredUserCommand> _handler;
    private Mock<IValidator<DeleteRegisteredUserCommand>> _validator;
    private Mock<IUserRepository> _userRepository;

    [SetUp]
    public void Arrange()
    {
        _validator = new Mock<IValidator<DeleteRegisteredUserCommand>>();
        _validator.Setup(x => x.Validate(It.IsAny<DeleteRegisteredUserCommand>()))
            .Returns(new ValidationResult());

        _userRepository = new Mock<IUserRepository>();
        _userRepository.Setup(x => x.DeleteUser(It.IsAny<string>())).Returns(() => Task.CompletedTask);

        _handler = new DeleteRegisteredUserCommandHandler(_validator.Object, _userRepository.Object);
    }

    [Test]
    public void ThenAnExceptionIsThrownIfTheCommandIsInvalid()
    {
        //Arrange
        _validator.Setup(x => x.Validate(It.IsAny<DeleteRegisteredUserCommand>()))
            .Returns(new ValidationResult(
                new List<ValidationFailure>
                {
                    new("TEST", "ERROR")
                }
            ));

        var command = new DeleteRegisteredUserCommand();

        //Act & Assert
        Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, new CancellationToken()));
    }

    [Test]
    public async Task ThenTheUserIsDeleted()
    {
        var command = new DeleteRegisteredUserCommand
        {
            UserRef = "TESTUSER"
        };

        await _handler.Handle(command, new CancellationToken());

        _userRepository.Verify(x => x.DeleteUser(It.Is<string>(s => s == "TESTUSER")), Times.Once);
    }
}