using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprenticeshipPriceHistory;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetApprenticeshipPriceHistory
{
    [TestFixture]
    public class WhenGettingApprenticeshipPriceHistory
    {
        private Mock<IProviderCommitmentsApi> _commitmentsApi;
        private GetApprenticeshipPriceHistoryQueryHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _commitmentsApi = new Mock<IProviderCommitmentsApi>();
            _commitmentsApi.Setup(x => x.GetPriceHistory(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(() => new List<PriceHistory>());

            _handler = new GetApprenticeshipPriceHistoryQueryHandler(_commitmentsApi.Object);
        }

        [Test]
        public async Task ThenTheApiIsCalledToRetrieveData()
        {
            //Arrange
            var query = new GetApprenticeshipPriceHistoryQueryRequest
            {
                ApprenticeshipId = 1
            };

            //Act
            await _handler.Handle(query, new CancellationToken());

            //Assert
            _commitmentsApi.Verify(x => x.GetPriceHistory(It.IsAny<long>(), It.Is<long>(id => id == 1)), Times.Once);
        }
    }
}
