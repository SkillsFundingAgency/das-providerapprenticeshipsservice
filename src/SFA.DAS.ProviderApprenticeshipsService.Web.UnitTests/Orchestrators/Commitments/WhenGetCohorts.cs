using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments;
using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenGetCohorts : ApprenticeshipValidationTestBase
    {
        public Task<GetCommitmentsQueryResponse> Commitments; 

        [SetUp]
        protected virtual void SetUp()
        {
            Commitments = Task.FromResult(TestData());
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentsQueryRequest>())).Returns(Commitments);
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetProviderAgreementQueryRequest>()))
                .Returns(Task.FromResult(new GetProviderAgreementQueryResponse { HasAgreement = ProviderAgreementStatus.Agreed }));

            base.SetUp();
        }

        [Test]
        public async Task TestHappyPath()
        {
            await _orchestrator.GetCohorts(1234567);

            _mockMediator.Verify(m => m.SendAsync(It.IsAny<GetCommitmentsQueryRequest>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task TestFilter()
        {
            SetUpOrchestrator();

            var result = await _orchestrator.GetCohorts(1234567);

            _mockMediator.Verify(m => m.SendAsync(It.IsAny<GetCommitmentsQueryRequest>()), Times.Once);

            result.WithEmployerCount.Should().Be(2);
            result.ReadyForReviewCount.Should().Be(6);
        }

        private GetCommitmentsQueryResponse TestData()
        {
            return new GetCommitmentsQueryResponse
                       {
                           Commitments = GetTestCommitmentsOfStatus(1, RequestStatus.NewRequest, RequestStatus.ReadyForApproval,
                               RequestStatus.ReadyForApproval, RequestStatus.WithEmployerForApproval, RequestStatus.WithEmployerForApproval,
                               RequestStatus.ReadyForReview, RequestStatus.ReadyForReview, RequestStatus.ReadyForReview).ToList()
                       };
        }
    }
}
