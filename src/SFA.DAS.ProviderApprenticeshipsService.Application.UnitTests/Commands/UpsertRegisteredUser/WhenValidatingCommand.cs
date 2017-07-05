using System.Linq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.UpsertRegisteredUser
{
    [TestFixture]
    public class WhenValidatingCommand
    {
        private UpsertRegisteredUserCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new UpsertRegisteredUserCommandValidator();
        }

        [Test]
        public void ThenCommandIsValidIfAllFieldsAreProvided()
        {
            //Arrange
            var command = new UpsertRegisteredUserCommand
            {
                UserRef = "UserRef",
                DisplayName = "Displayname",
                Email = "Email",
                Ukprn = 12345
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
            var command = new UpsertRegisteredUserCommand
            {
                DisplayName = "Displayname",
                Email = "Email",
                Ukprn = 12345
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Contains(nameof(UpsertRegisteredUserCommand.UserRef))));
        }

        [Test]
        public void ThenEmailIsMandatory()
        {
            //Arrange
            var command = new UpsertRegisteredUserCommand
            {
                UserRef = "UserRef",
                DisplayName = "Displayname",
                Ukprn = 12345
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Contains(nameof(UpsertRegisteredUserCommand.Email))));
        }

        [Test]
        public void ThenUkprnisMandatory()
        {
            //Arrange
            var command = new UpsertRegisteredUserCommand
            {
                UserRef = "UserRef",
                DisplayName = "Displayname",
                Email = "Email"
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Contains(nameof(UpsertRegisteredUserCommand.Ukprn))));
        }

        [Test]
        public void ThenDisplayNameIsMandatory()
        {
            //Arrange
            var command = new UpsertRegisteredUserCommand
            {
                UserRef = "UserRef",
                Email = "Email",
                Ukprn = 12345
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Contains(nameof(UpsertRegisteredUserCommand.DisplayName))));
        }
    }
}
