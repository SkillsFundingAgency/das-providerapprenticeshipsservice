using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderHasRelationshipWithPermission;
using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenGetCohorts : ApprenticeshipValidationTestBase
    {
        private const long ValidTransferSenderId = 1L;
        private const string ProviderLastUpdatedName = "Test Provider User";

        [SetUp]
        protected void GetCohortsSetup()
        {
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCommitmentsQueryRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData());
            _mockMediator.Setup(m => m.Send(It.IsAny<GetProviderAgreementQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetProviderAgreementQueryResponse { HasAgreement = ProviderAgreementStatus.Agreed });

            _mockMediator.Setup(x => x.Send(It.IsAny<GetProviderHasRelationshipWithPermissionQueryRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetProviderHasRelationshipWithPermissionQueryResponse { HasPermission = true });

            SetUp();
        }

        [Test]
        public async Task TestHappyPath()
        {
            await _orchestrator.GetCohorts(1234567);

            _mockMediator.Verify(m => m.Send(It.IsAny<GetCommitmentsQueryRequest>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task ThenAllCountsShouldBeZeroIfNoCommitments()
        {
            //Arrange
            _mockMediator.Setup(x => x.Send(It.IsAny<GetCommitmentsQueryRequest>(), It.IsAny<CancellationToken>()))
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
            Assert.AreEqual(0, result.DraftCount);
        }
        
        [Test]
        public async Task ThenDraftsAreNotShownIfProviderHasNoPermissions()
        {
            //Arrange
            _mockMediator.Setup(x => x.Send(It.IsAny<GetProviderHasRelationshipWithPermissionQueryRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetProviderHasRelationshipWithPermissionQueryResponse { HasPermission = false });

            _mockMediator.Setup(x => x.Send(It.IsAny<GetCommitmentsQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCommitmentsQueryResponse
                {
                    Commitments = new List<CommitmentListItem>()
                });

            //Act
            var result = await _orchestrator.GetCohorts(1234567);

            //Assert
            Assert.IsFalse(result.ShowDrafts);
        }

        [TestCase(/*expectedReadyForReviewCount=*/0, /*expectedWithEmployerCount=*/0, /*expectedTransferFundedCohortsCount=*/0, /*expectedDraftCount=*/0,
            null, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.EmployerOnly, LastAction.None, 0, false, CommitmentStatus.New, TestName = "Just created by an employer")]
        [TestCase(/*expectedReadyForReviewCount=*/1, /*expectedWithEmployerCount=*/0, /*expectedTransferFundedCohortsCount=*/0, /*expectedDraftCount=*/0,
            null, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.ProviderOnly, LastAction.Amend, 0, false, CommitmentStatus.Active, TestName = "Been sent to provider by employer to add apprentices")]
        [TestCase(/*expectedReadyForReviewCount=*/0, /*expectedWithEmployerCount=*/1, /*expectedTransferFundedCohortsCount=*/0, /*expectedDraftCount=*/0,
            ValidTransferSenderId, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.EmployerOnly, LastAction.Amend, 0, false, CommitmentStatus.Active, TestName = "With receiving employer")]
        [TestCase(/*expectedReadyForReviewCount=*/1, /*expectedWithEmployerCount=*/0, /*expectedTransferFundedCohortsCount=*/0, /*expectedDraftCount=*/0,
            ValidTransferSenderId, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.ProviderOnly, LastAction.Amend, 0, true, CommitmentStatus.Active, TestName = "With provider")]
        [TestCase(/*expectedReadyForReviewCount=*/0, /*expectedWithEmployerCount=*/0, /*expectedTransferFundedCohortsCount=*/1, /*expectedDraftCount=*/0,
            ValidTransferSenderId, TransferApprovalStatus.Pending, AgreementStatus.BothAgreed,
            EditStatus.Both, LastAction.Approve, 1, true, CommitmentStatus.Active, TestName = "With sender but not yet actioned by them")]
        [TestCase(/*expectedReadyForReviewCount=*/0, /*expectedWithEmployerCount=*/1, /*expectedTransferFundedCohortsCount=*/0, /*expectedDraftCount=*/0,
            ValidTransferSenderId, TransferApprovalStatus.Rejected, AgreementStatus.NotAgreed,
            EditStatus.EmployerOnly, LastAction.Amend, 1, true, CommitmentStatus.Active, TestName = "With sender, rejected by them, but not yet saved or edited")]
        [TestCase(/*expectedReadyForReviewCount=*/0, /*expectedWithEmployerCount=*/0, /*expectedTransferFundedCohortsCount=*/0, /*expectedDraftCount=*/0,
            ValidTransferSenderId, TransferApprovalStatus.Approved, AgreementStatus.BothAgreed,
            EditStatus.Both, LastAction.Approve, 1, true, CommitmentStatus.Active, TestName = "Approved by all 3 parties")]
        [TestCase(/*expectedReadyForReviewCount=*/0, /*expectedWithEmployerCount=*/0, /*expectedTransferFundedCohortsCount=*/0, /*expectedDraftCount=*/1,
            ValidTransferSenderId, TransferApprovalStatus.Pending, AgreementStatus.NotAgreed,
            EditStatus.ProviderOnly, LastAction.None, 1, true, CommitmentStatus.New, TestName = "Provider draft")]

        public async Task ThenCountsShouldBeCorrectWhenEmployerHasASingleCommitmentThats(
            int expectedReadyForReviewCount, int expectedWithEmployerCount, int expectedTransferFundedCohortsCount, int expectedDraftCount,
            long? transferSenderId, TransferApprovalStatus transferApprovalStatus,
            AgreementStatus agreementStatus, EditStatus editStatus, LastAction lastAction, int apprenticeshipCount,
            bool supplyProviderLastUpdatedName, CommitmentStatus commitmentStatus)
        {
            //Arrange
            _mockMediator.Setup(x => x.Send(It.IsAny<GetCommitmentsQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCommitmentsQueryResponse
                {
                    Commitments = commitmentStatus == CommitmentStatus.Active || editStatus == EditStatus.ProviderOnly ? new List <CommitmentListItem>
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
            Assert.AreEqual(expectedDraftCount, result.DraftCount, "Incorrect DraftCount");
        }

        [Test]
        public async Task TestFilter()
        {
            SetUpOrchestrator();

            var result = await _orchestrator.GetCohorts(1234567);

            _mockMediator.Verify(m => m.Send(It.IsAny<GetCommitmentsQueryRequest>(), new CancellationToken()), Times.Once);

            result.DraftCount.Should().Be(1);
            result.WithEmployerCount.Should().Be(2);
            result.ReadyForReviewCount.Should().Be(5);
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
