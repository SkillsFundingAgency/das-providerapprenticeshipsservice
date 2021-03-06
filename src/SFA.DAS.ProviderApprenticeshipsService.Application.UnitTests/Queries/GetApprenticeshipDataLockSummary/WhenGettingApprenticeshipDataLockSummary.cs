﻿using System.Threading;
using System.Threading.Tasks;
using Moq;

using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipDataLockSummary;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetApprenticeshipDataLockSummary
{
    [TestFixture]
    public class WhenGettingApprenticeshipDataLockSummary
    {
        private GetApprenticeshipDataLockSummaryQueryHandler _handler;
        private Mock<IProviderCommitmentsApi> _commitmentsApi;


        [SetUp]
        public void Arrange()
        {
            _commitmentsApi = new Mock<IProviderCommitmentsApi>();
            _commitmentsApi.Setup(x => x.GetDataLockSummary(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(new DataLockSummary());

            _handler = new GetApprenticeshipDataLockSummaryQueryHandler(_commitmentsApi.Object);
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
            await _handler.Handle(request, new CancellationToken());

            //Assert
            _commitmentsApi.Verify(x => x.GetDataLockSummary(It.IsAny<long>(), It.IsAny<long>()), Times.Once);
        }
    }
}
