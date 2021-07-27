using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenGettingApprovedViewModel : ApprenticeshipValidationTestBase
    {
        private CommitmentView _commitment;
        private List<CommitmentListItem> _otherCommitments;

        public override void SetUp()
        {
            _commitment = new CommitmentView
            {
                Messages = new List<MessageView>()
            };

            _otherCommitments = new List<CommitmentListItem>();

            _mockMediator.Setup(x => x.Send(It.IsAny<GetCommitmentQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetCommitmentQueryResponse
                {
                    Commitment = _commitment
                });

            _mockMediator.Setup(x => x.Send(It.IsAny<GetCommitmentsQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetCommitmentsQueryResponse
                {
                    Commitments = _otherCommitments
                });

            base.SetUp();
        }

        [Test]
        public async Task ThenARequestForTheCommitmentDataIsMade()
        {
            await _orchestrator.GetApprovedViewModel(1, "Hashed-Id");

            _mockMediator.Verify(x => x.Send(It.IsAny<GetCommitmentQueryRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task ThenTheViewModelShouldIndicateWhenThereAreOtherCohortsInSameStatus()
        {
            _otherCommitments.Add(new CommitmentListItem
            {
                ApprenticeshipCount = 10,
                AgreementStatus = AgreementStatus.EmployerAgreed,
                CanBeApproved = true,
                EditStatus = EditStatus.ProviderOnly,
                LastAction = LastAction.Approve,
                ProviderLastUpdateInfo = new LastUpdateInfo {Name = "Test"}
            });

            var result = await _orchestrator.GetApprovedViewModel(1, "Hashed-Id");

            Assert.IsTrue(result.HasOtherCohortsAwaitingApproval);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenTheViewModelShouldMapShowApprenticeEmail(bool expected)
        {
            _mockCommitmentsV2Service.Setup(x => x.ApprenticeEmailRequired(1)).ReturnsAsync(expected);
            
            var result = await _orchestrator.GetApprovedViewModel(1, "Hashed-Id");

            Assert.AreEqual(expected, result.ShowApprenticeEmail);
        }

        [Test]
        public async Task ThenTheViewModelShouldIndicateWhenThereAreNotOtherCohortsInSameStatus()
        {
            var result = await _orchestrator.GetApprovedViewModel(1, "Hashed-Id");

            Assert.IsFalse(result.HasOtherCohortsAwaitingApproval);
        }

        [TestCase(false, "Cohort approved")]
        [TestCase(true, "Cohort approved")]
        public async Task ThenTheHeadlineShouldShouldBeCohortApproved(bool isTransfer, string expectHeadline)
        {
            _commitment.TransferSender = (isTransfer ? new TransferSender { Id = 1L} : null);

            var result = await _orchestrator.GetApprovedViewModel(1, "Hashed-Id");

            Assert.AreEqual(expectHeadline, result.Headline);
        }

        [Test]
        public async Task ThenTheViewModelShouldIndicateWhenCohortIsForTransfer()
        {
            _commitment.TransferSender = new TransferSender { Id = 1L };

            var result = await _orchestrator.GetApprovedViewModel(1, "Hashed-Id");

            Assert.IsTrue(result.IsTransfer);
        }

        [Test]
        public async Task ThenTheViewModelShouldIndicateWhenCohortIsNotForTransfer()
        {
            _commitment.TransferSender = null;

            var result = await _orchestrator.GetApprovedViewModel(1, "Hashed-Id");

            Assert.IsFalse(result.IsTransfer);
        }
    }
}
