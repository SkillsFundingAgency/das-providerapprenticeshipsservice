using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.ApprovedApprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetApprovedApprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetApprovedApprenticeship
{
    [TestFixture]
    public class WhenIGetApprovedApprenticeship
    {
        private GetApprovedApprenticeshipQueryHandler _handler;
        private GetApprovedApprenticeshipQueryRequest _validRequest;
        private ApprovedApprenticeship _apiResult;
        private Mock<IProviderCommitmentsApi> _mockCommitmentsApi;
        private long _expectedProviderId;
        private long _expectedApprovedApprenticeshipId;
        private GetApprovedApprenticeshipQueryResponse _result;

        [SetUp]
        public async Task GivenAGetApprovedApprenticeshipQueryHandler_WhenCallingHandle()
        {
            _apiResult = new ApprovedApprenticeship();

            _mockCommitmentsApi = new Mock<IProviderCommitmentsApi>();
            _mockCommitmentsApi.Setup(x => x.GetApprovedApprenticeship(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(_apiResult);

            _expectedProviderId = 112;
            _expectedApprovedApprenticeshipId = 114;

            _validRequest = new GetApprovedApprenticeshipQueryRequest
            {
                ProviderId = _expectedProviderId,
                ApprovedApprenticeshipId = _expectedApprovedApprenticeshipId
            };

            _handler = new GetApprovedApprenticeshipQueryHandler(_mockCommitmentsApi.Object);
            
            _result = await _handler.Handle(_validRequest);
        }

        [Test]
        public void ThenTheApprovedApprenticeshipReturnedByTheApiIsRetrieved()
        {
            Assert.That(_result.ApprovedApprenticeship, Is.EqualTo(_apiResult));
        }

        [Test]
        public void ThenTheApiIsCalledWithTheCorrectIds()
        {
            _mockCommitmentsApi.Verify(x => x.GetApprovedApprenticeship(_expectedProviderId, _expectedApprovedApprenticeshipId));
        }

    }
}
