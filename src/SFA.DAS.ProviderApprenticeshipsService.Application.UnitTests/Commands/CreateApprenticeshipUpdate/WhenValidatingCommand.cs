using System.Linq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.CreateApprenticeshipUpdate
{
    [TestFixture]
    public class WhenValidatingCommand
    {
        private CreateApprenticeshipUpdateCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new CreateApprenticeshipUpdateCommandValidator();
        }

        [Test]
        public void ThenUserIdIsMandatory()
        {
            //Arrange
            var command = new CreateApprenticeshipUpdateCommand
            {
                ApprenticeshipUpdate = new ApprenticeshipUpdate()
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x=> x.PropertyName.Contains(nameof(CreateApprenticeshipUpdateCommand.UserId))));
        }

        [Test]
        public void ThenProviderIdIsMandatory()
        {
            //Arrange
            var command = new CreateApprenticeshipUpdateCommand
            {
                ApprenticeshipUpdate = new ApprenticeshipUpdate()
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Contains(nameof(CreateApprenticeshipUpdateCommand.ProviderId))));
        }

        [Test]
        public void ThenApprenticeshipIdUpdateIsMandatory()
        {
            //Arrange
            var command = new CreateApprenticeshipUpdateCommand
            {
                ApprenticeshipUpdate = new ApprenticeshipUpdate()
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Contains(nameof(ApprenticeshipUpdate.ApprenticeshipId))));
        }

        [Test]
        public void ThenIfAllMandatoryPropertiesAreProvidedThenIsValid()
        {
            //Arrange
            var command = new CreateApprenticeshipUpdateCommand
            {
                UserId = "User1",
                ProviderId = 2,
                ApprenticeshipUpdate = new ApprenticeshipUpdate
                {
                    ApprenticeshipId = 3,
                    Originator = Originator.Employer
                }
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsTrue(result.IsValid);
        }
    }
}
