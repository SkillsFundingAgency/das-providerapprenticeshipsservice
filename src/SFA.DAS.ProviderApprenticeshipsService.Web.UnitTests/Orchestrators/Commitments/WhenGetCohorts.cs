using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments;
using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenGetCohorts : ApprenticeshipValidationTestBase
    {
        [SetUp]
        protected void GetCohortsSetup()
        {
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentsQueryRequest>())).ReturnsAsync(TestData());
            _mockMediator.Setup(m => m.SendAsync(It.IsAny<GetProviderAgreementQueryRequest>()))
                .ReturnsAsync(new GetProviderAgreementQueryResponse { HasAgreement = ProviderAgreementStatus.Agreed });

            SetUp();
        }

        [Test]
        public async Task TestHappyPath()
        {
            await _orchestrator.GetCohorts(1234567);

            _mockMediator.Verify(m => m.SendAsync(It.IsAny<GetCommitmentsQueryRequest>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task ThenAllCountsShouldBeZeroIfNoCommitments()
        {
            //Arrange
            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentsQueryRequest>()))
                .ReturnsAsync(new GetCommitmentsQueryResponse
                {
                    Commitments = new List<CommitmentListItem>()
                });

            //Act
            var result = await _orchestrator.GetCohorts(1234567);

            //Assert
            Assert.AreEqual(0, result.ReadyForReviewCount);
            Assert.AreEqual(0, result.WithEmployerCount);
            Assert.AreEqual(0, result.TransferFundedCohortsCount);
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
