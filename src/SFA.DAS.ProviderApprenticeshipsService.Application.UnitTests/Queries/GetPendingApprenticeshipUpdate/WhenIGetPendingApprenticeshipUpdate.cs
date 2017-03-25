using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetPendingApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetPendingApprenticeshipUpdate
{
    public class WhenIGetPendingApprenticeshipUpdate
    {
        private GetPendingApprenticeshipUpdateQueryHandler _handler;
        private Mock<IProviderCommitmentsApi> _clientApi;

        [SetUp]
        public void Arrange()
        {
            _clientApi = new Mock<IProviderCommitmentsApi>();
            _clientApi.Setup(x => x.GetPendingApprenticeshipUpdate(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(new ApprenticeshipUpdate());

            _handler = new GetPendingApprenticeshipUpdateQueryHandler(_clientApi.Object);
        }

        [Test]
        public async Task ThenTheApiShouldBeCalledToGetData()
        {
            //Arrange
            var request = new GetPendingApprenticeshipUpdateQueryRequest
            {
                ApprenticeshipId = 1,
                ProviderId = 1
            };

            //Act
            await _handler.Handle(request);

            //Assert
            _clientApi.Verify(x=>x.GetPendingApprenticeshipUpdate(It.IsAny<long>(), It.IsAny<long>()), Times.Once);
        }
    }
}
