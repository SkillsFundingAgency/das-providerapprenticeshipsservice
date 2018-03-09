using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
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

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(() => new GetCommitmentQueryResponse
                {
                    Commitment = _commitment
                });

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentsQueryRequest>()))
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

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()),
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
                EditStatus = DAS.Commitments.Api.Types.Commitment.Types.EditStatus.ProviderOnly,
                LastAction = DAS.Commitments.Api.Types.Commitment.Types.LastAction.Approve
            });

            //this mock shoulkd return ReadyForApproval for all calls to GetStatus
            //_statusCalculator 

            var result = await _orchestrator.GetApprovedViewModel(1, "Hashed-Id");

            Assert.IsTrue(result.HasOtherCohortsAwaitingApproval);
        }

        [Test]
        public async Task ThenTheViewModelShouldIndicateWhenThereAreNotOtherCohortsInSameStatus()
        {
            var result = await _orchestrator.GetApprovedViewModel(1, "Hashed-Id");

            Assert.IsFalse(result.HasOtherCohortsAwaitingApproval);
        }

        [TestCase(false, "Cohort approved")]
        [TestCase(true, "Cohort approved and transfer request sent")]
        public async Task ThenTheHeadlineShouldReflectWhetherTheCommitmentIsToBePaidByTransfer(bool isTransfer, string expectHeadline)
        {
            _commitment.TransferSenderId = isTransfer ? 1L : default(long?);

            var result = await _orchestrator.GetApprovedViewModel(1, "Hashed-Id");

            Assert.AreEqual(expectHeadline, result.Headline);
        }

        [Test]
        public async Task ThenTheViewModelShouldIndicateWhenCohortIsForTransfer()
        {
            _commitment.TransferSenderId = 1L;

            var result = await _orchestrator.GetApprovedViewModel(1, "Hashed-Id");

            Assert.IsTrue(result.IsTransfer);
        }

        [Test]
        public async Task ThenTheViewModelShouldIndicateWhenCohortIsNotForTransfer()
        {
            _commitment.TransferSenderId = null;

            var result = await _orchestrator.GetApprovedViewModel(1, "Hashed-Id");

            Assert.IsFalse(result.IsTransfer);
        }
    }
}
