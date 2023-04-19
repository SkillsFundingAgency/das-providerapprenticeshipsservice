using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteRegisteredUser;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.DeleteRegisteredUser;

[TestFixture]
public class WhenValidatingCommand
{
    private DeleteRegisteredUserCommandValidator _validator;

    [SetUp]
    public void Arrange()
    {
        _validator = new DeleteRegisteredUserCommandValidator();
    }

    [Test]
    public void ThenCommandIsValidIfAllFieldsAreProvided()
    {
        //Arrange
        var command = new DeleteRegisteredUserCommand
        {
            UserRef = "UserRef"
        };

        //Act
        var result = _validator.Validate(command);

        //Assert
        Assert.IsTrue(result.IsValid);
    }

    [Test]
    public void ThenUserRefIsMandatory()
    {
        //Arrange
        var command = new DeleteRegisteredUserCommand();

        //Act
        var result = _validator.Validate(command);

        //Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Contains(nameof(DeleteRegisteredUserCommand.UserRef))));
    }
}