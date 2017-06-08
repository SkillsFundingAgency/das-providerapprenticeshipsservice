using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLocks;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetApprenticeshipDataLocks
{
    [TestFixture]
    public class WhenGettingApprenticeshipDataLocks
    {
        private GetApprenticeshipDataLocksHandler _handler;
        private Mock<IDataLockApi> _dataLockApi;

        [SetUp]
        public void Arrange()
        {
            _dataLockApi = new Mock<IDataLockApi>();
            _dataLockApi.Setup(x => x.GetDataLocks(It.IsAny<long>()))
                .ReturnsAsync(new List<DataLockStatus>());

            _handler = new GetApprenticeshipDataLocksHandler(_dataLockApi.Object);
        }

        [Test]
        public async Task ThenTheApiIsCalledToRetrieveDataLocks()
        {
            //Arrange
            var request = new GetApprenticeshipDataLocksRequest
            {
                ApprenticeshipId = 1
            };

            //Act
            await _handler.Handle(request);

            //Assert
            _dataLockApi.Verify(x => x.GetDataLocks(It.IsAny<long>()), Times.Once);
        }
    }
}
