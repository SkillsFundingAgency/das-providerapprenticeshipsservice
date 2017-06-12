using System;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.TriageApprenticeshipDataLocks;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.TriageApprenticeshipDataLocks
{
    [TestFixture]
    public class WhenTriagingApprenticeshipDataLocks
    {
        private TriageApprenticeshipDataLocksCommandHandler _handler;
        private Mock<IDataLockApi> _dataLockApi;
        private Mock<ILog> _logger;

        [SetUp]
        public void Arrange()
        {
            _dataLockApi = new Mock<IDataLockApi>();
            //_dataLockApi.Setup(x => x.) //todo: set something up here

            _logger = new Mock<ILog>();

            _handler = new TriageApprenticeshipDataLocksCommandHandler(_dataLockApi.Object, _logger.Object);
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
            var result = await _handler.Handle(command);

            //Assert
            throw new NotImplementedException();
            //todo: verify api was called
        }
    }
}
