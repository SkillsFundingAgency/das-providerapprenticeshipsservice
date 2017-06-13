using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLocks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetApprenticeshipDataLockSummary
{
    [TestFixture]
    public class WhenGettingApprenticeshipDataLockSummary
    {
        private GetApprenticeshipDataLockSummaryQueryHandler _handler;
        private Mock<IDataLockApi> _dataLockApi;

        [SetUp]
        public void Arrange()
        {
            _dataLockApi = new Mock<IDataLockApi>();
            _dataLockApi.Setup(x => x.GetDataLockSummary(It.IsAny<long>()))
                .ReturnsAsync(new List<DataLockSummary>());
            //todo: this will not be a list after package update

            _handler = new GetApprenticeshipDataLockSummaryQueryHandler(_dataLockApi.Object);
        }

        [Test]
        public async Task ThenTheApiIsCalledToRetrieveDataLocks()
        {
            //Arrange
            var request = new GetApprenticeshipDataLockSummaryQueryRequest
            {
                ApprenticeshipId = 1
            };

            //Act
            await _handler.Handle(request);

            //Assert
            _dataLockApi.Verify(x => x.GetDataLockSummary(It.IsAny<long>()), Times.Once);
        }
    }
}
