using System.Linq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateRelationship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.UpdateRelationship
{
    [TestFixture]
    public class WhenValidatingCommand
    {
        private UpdateRelationshipCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new UpdateRelationshipCommandValidator();
        }

        [Test]
        public void ThenProviderIdIsMandatory()
        {
            //Arrange
            var command = new UpdateRelationshipCommand();

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e=> e.PropertyName == "ProviderId"));
        }

        [Test]
        public void ThenUserIdIsMandatory()
        {
            //Arrange
            var command = new UpdateRelationshipCommand();

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "UserId"));
        }

        [Test]
        public void ThenRelationshipIsMandatory()
        {
            //Arrange
            var command = new UpdateRelationshipCommand();

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Relationship"));
        }

    }
}
