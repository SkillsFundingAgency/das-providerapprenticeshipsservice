using System.Linq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.UpsertRegisteredUserCommandHandler
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
                UserId = "UserId",
                DisplayName = "Displayname",
                Email = "Email",
                Ukprn = "Ukprn"
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void ThenUserIdIsMandatory()
        {
            //Arrange
            var command = new UpsertRegisteredUserCommand
            {
                DisplayName = "Displayname",
                Email = "Email",
                Ukprn = "Ukprn"
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Contains(nameof(UpsertRegisteredUserCommand.UserId))));
        }

        [Test]
        public void ThenEmailIsMandatory()
        {
            //Arrange
            var command = new UpsertRegisteredUserCommand
            {
                UserId = "UserId",
                DisplayName = "Displayname",
                Ukprn = "Ukprn"
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
                UserId = "UserId",
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
                UserId = "UserId",
                Email = "Email",
                Ukprn = "Ukprn"
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Contains(nameof(UpsertRegisteredUserCommand.DisplayName))));
        }
    }
}
