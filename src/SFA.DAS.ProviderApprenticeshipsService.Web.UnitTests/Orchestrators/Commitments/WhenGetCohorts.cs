using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments;
using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenGetCohorts : ApprenticeshipValidationTestBase
    {
        private const long ValidTransferSenderId = 1L;
        private const string ProviderLastUpdatedName = "Anna-Leigh Probin";

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

        [TestCase(/*expectedReadyForReviewCount=*/0, /*expectedWithEmployerCount=*/0, /*expectedTransferFundedCohortsCount=*/0,
            null, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.EmployerOnly, LastAction.None, 0, false, CommitmentStatus.New, TestName = "Just created by an employer")]
        [TestCase(/*expectedReadyForReviewCount=*/1, /*expectedWithEmployerCount=*/0, /*expectedTransferFundedCohortsCount=*/0,
            null, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.ProviderOnly, LastAction.None, 0, false, CommitmentStatus.Active, TestName = "Been sent to provider by employer to add apprentices")]
        [TestCase(/*expectedReadyForReviewCount=*/0, /*expectedWithEmployerCount=*/1, /*expectedTransferFundedCohortsCount=*/0,
            ValidTransferSenderId, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.EmployerOnly, LastAction.Amend, 0, false, CommitmentStatus.Active, TestName = "With receiving employer")]
        [TestCase(/*expectedReadyForReviewCount=*/1, /*expectedWithEmployerCount=*/0, /*expectedTransferFundedCohortsCount=*/0,
            ValidTransferSenderId, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.ProviderOnly, LastAction.Amend, 0, true, CommitmentStatus.Active, TestName = "With provider")]
        [TestCase(/*expectedReadyForReviewCount=*/0, /*expectedWithEmployerCount=*/0, /*expectedTransferFundedCohortsCount=*/1,
            ValidTransferSenderId, TransferApprovalStatus.Pending, AgreementStatus.BothAgreed,
            EditStatus.Both, LastAction.Approve, 1, true, CommitmentStatus.Active, TestName = "With sender but not yet actioned by them")]
        [TestCase(/*expectedReadyForReviewCount=*/0, /*expectedWithEmployerCount=*/0, /*expectedTransferFundedCohortsCount=*/1,
            ValidTransferSenderId, TransferApprovalStatus.Rejected, AgreementStatus.NotAgreed,
            EditStatus.EmployerOnly, LastAction.Amend, 1, true, CommitmentStatus.Active, TestName = "With sender, rejected by them, but not yet saved or edited")]
        [TestCase(/*expectedReadyForReviewCount=*/0, /*expectedWithEmployerCount=*/0, /*expectedTransferFundedCohortsCount=*/0,
            ValidTransferSenderId, TransferApprovalStatus.Approved, AgreementStatus.BothAgreed,
            EditStatus.Both, LastAction.Approve, 1, true, CommitmentStatus.Active, TestName = "Approved by all 3 parties")]
        public async Task ThenCountsShouldBeCorrectWhenEmployerHasASingleCommitmentThats(
            int expectedReadyForReviewCount, int expectedWithEmployerCount, int expectedTransferFundedCohortsCount,
            long? transferSenderId, TransferApprovalStatus transferApprovalStatus,
            AgreementStatus agreementStatus, EditStatus editStatus, LastAction lastAction, int apprenticeshipCount,
            bool supplyProviderLastUpdatedName, CommitmentStatus commitmentStatus)
        {
            //Arrange
            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentsQueryRequest>()))
                .ReturnsAsync(new GetCommitmentsQueryResponse
                {
                    Commitments = commitmentStatus == CommitmentStatus.Active ? new List <CommitmentListItem>
                    {
                        new CommitmentListItem
                        {
                            CommitmentStatus = commitmentStatus,
                            TransferSenderId = transferSenderId,
                            TransferApprovalStatus = transferApprovalStatus,
                            AgreementStatus = agreementStatus,
                            EditStatus = editStatus,
                            LastAction = lastAction,
                            ApprenticeshipCount = apprenticeshipCount,
                            ProviderLastUpdateInfo = supplyProviderLastUpdatedName ? new LastUpdateInfo {Name = ProviderLastUpdatedName} : null
                        }
                    }
                        : new List<CommitmentListItem>()
                });

            //Act
            var result = await _orchestrator.GetCohorts(1234567);

            //Assert
            Assert.AreEqual(expectedReadyForReviewCount, result.ReadyForReviewCount, "Incorrect ReadyForReviewCount");
            Assert.AreEqual(expectedWithEmployerCount, result.WithEmployerCount, "Incorrect WithEmployerCount");
            Assert.AreEqual(expectedTransferFundedCohortsCount, result.TransferFundedCohortsCount, "Incorrect TransferFundedCohortsCount");
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
