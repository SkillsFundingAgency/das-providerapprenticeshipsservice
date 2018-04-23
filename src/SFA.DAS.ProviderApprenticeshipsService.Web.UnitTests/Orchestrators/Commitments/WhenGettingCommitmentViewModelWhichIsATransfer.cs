using System.Collections.Generic;

using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;
using CommitmentView = SFA.DAS.Commitments.Api.Types.Commitment.CommitmentView;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    using Task = System.Threading.Tasks.Task;

    public class WhenGettingCommitmentViewModelWhichIsATransfer : WhenGettingCommitmentViewModel
    {
        [Test(Description = "Should return ReadOnly if Both Parties have agreed")]
        public async Task ShouldBeAbleToViewACommitmentWhereBothPartiesHaveAgreed()
        {
            var commitment = new CommitmentView
            {
                AgreementStatus = AgreementStatus.BothAgreed,
                EditStatus = EditStatus.Both,
                Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship (),
                    new Apprenticeship ()
                },
                Messages = new List<MessageView>()
            };

            _mockMediator = GetMediator(commitment);
            SetUpOrchestrator();
            var result = await _orchestrator.GetCommitmentDetails(1L, "ABBA123");

            result.IsReadOnly.Should().Be(true);
        }

        [TestCase(TransferApprovalStatus.Pending, EditStatus.Both, RequestStatus.WithSenderForApproval, Description = "Should return a Request status of WithSenderForApproval")]
        [TestCase(TransferApprovalStatus.Rejected, EditStatus.EmployerOnly, RequestStatus.RejectedBySender, Description = "Should return a Request status of Pending  RejectedBySender")]
        public void ShouldReturnARequestStatus(TransferApprovalStatus transferApprovalStatus, EditStatus editStatus, RequestStatus requestStatus)
        {
            var commitment = new CommitmentView
            {
                TransferSender = new TransferSender { Id =1, Name = "Name", TransferApprovalStatus = transferApprovalStatus },
                AgreementStatus = AgreementStatus.BothAgreed,
                EditStatus = editStatus,
                Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship (),
                    new Apprenticeship ()
                },
                Messages = new List<MessageView>()
            };

            _mockMediator = GetMediator(commitment);
            SetUpOrchestrator();
            var result = _orchestrator.GetCommitmentDetails(1L, "ABBA213").Result;

            result.Status.Should().Be(requestStatus);
        }

    }
}
