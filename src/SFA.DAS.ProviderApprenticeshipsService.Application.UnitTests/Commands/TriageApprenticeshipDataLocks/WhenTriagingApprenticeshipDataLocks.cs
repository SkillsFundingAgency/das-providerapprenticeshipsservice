using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.TriageApprenticeshipDataLocks;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.TriageApprenticeshipDataLocks
{
    [TestFixture]
    public class WhenTriagingApprenticeshipDataLocks
    {
        private TriageApprenticeshipDataLocksCommandHandler _handler;
        private Mock<IProviderCommitmentsApi> _commitmentsApi;
        private Mock<ILog> _logger;
        private Mock<IValidator<TriageApprenticeshipDataLocksCommand>> _validator;

        [SetUp]
        public void Arrange()
        {
            _commitmentsApi = new Mock<IProviderCommitmentsApi>();
            _commitmentsApi.Setup(x => x.PatchDataLocks(It.IsAny<long>(), It.IsAny<long>(),
                It.IsAny<DataLockTriageSubmission>()))
                .Returns(() => Task.FromResult(new Unit()));

            _validator = new Mock<IValidator<TriageApprenticeshipDataLocksCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<TriageApprenticeshipDataLocksCommand>()))
                .Returns(() => new ValidationResult());

            _logger = new Mock<ILog>();

            _handler = new TriageApprenticeshipDataLocksCommandHandler(_commitmentsApi.Object, _logger.Object, _validator.Object);
        }

        [Test]
        public async Task ThenTheApiIsCalledToTriageDataLocks()
        {
            //Arrange
            var command = new TriageApprenticeshipDataLocksCommand
            {
                ApprenticeshipId = 1,
                TriageStatus = TriageStatus.Change,
                UserId = "USER"
            };

            //Act
            await _handler.Handle(command);

            //Assert
            _commitmentsApi.Verify(x => x.PatchDataLocks(It.IsAny<long>(), It.IsAny<long>(),
                It.IsAny<DataLockTriageSubmission>()), Times.Once());
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Arrange
            var command = new TriageApprenticeshipDataLocksCommand
            {
                ApprenticeshipId = 1,
                TriageStatus = TriageStatus.Change,
                UserId = "USER"
            };

            //Act
            await _handler.Handle(command);

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<TriageApprenticeshipDataLocksCommand>()), Times.Once);
        }
    }
}
