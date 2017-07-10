using System;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.TriageApprenticeshipDataLocks;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.TriageApprenticeshipDataLocks
{
    [TestFixture]
    public class WhenValidatingCommand
    {
        private TriageApprenticeshipDataLocksCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new TriageApprenticeshipDataLocksCommandValidator();
        }

        [Test]
        public void ThenApprenticeshipIdIsMandatory()
        {
            //Arrange
            var command = new TriageApprenticeshipDataLocksCommand();

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(
                result.Errors.Any(
                    x => x.PropertyName.Contains(nameof(TriageApprenticeshipDataLocksCommand.ApprenticeshipId))));
        }

        [Test]
        public void ThenTriageStatusIsMandatory()
        {
            //Arrange
            var command = new TriageApprenticeshipDataLocksCommand
            {
                TriageStatus = TriageStatus.Unknown - 1
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(
                result.Errors.Any(
                    x => x.PropertyName.Contains(nameof(TriageApprenticeshipDataLocksCommand.TriageStatus))));
        }

        [Test]
        public void ThenUserIdMandatory()
        {
            //Arrange
            var command = new TriageApprenticeshipDataLocksCommand();

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(
                result.Errors.Any(
                    x => x.PropertyName.Contains(nameof(TriageApprenticeshipDataLocksCommand.UserId))));
        }

        [Test]
        public void ThenCommandIsValidWhenAllPropertiesAreProvided()
        {
            //Arrange
            var command = new TriageApprenticeshipDataLocksCommand
            {
                ApprenticeshipId = 1,
                ProviderId = 666,
                TriageStatus = TriageStatus.Change,
                UserId = "TEST"
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsTrue(result.IsValid);   
        }
    }
}
