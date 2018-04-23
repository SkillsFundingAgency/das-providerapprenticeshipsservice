using System;
using System.Collections.Generic;

using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetFrameworks;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetRelationshipByCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetStandards;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using CommitmentView = SFA.DAS.Commitments.Api.Types.Commitment.CommitmentView;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    using Task = System.Threading.Tasks.Task;

    public class WhenGettingCommitmentViewModelWhichIsATransfer : ApprenticeshipValidationTestBase
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

        // --- Helpers ---

        private static Mock<IMediator> GetMediator(CommitmentView commitment)
        {
            var respons = new GetCommitmentQueryResponse
            {
                Commitment = commitment
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .Returns(Task.Factory.StartNew(() => respons));

            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetStandardsQueryRequest>()))
                .ReturnsAsync(() => new GetStandardsQueryResponse
                {
                    Standards = new List<Standard>()
                });

            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetFrameworksQueryRequest>()))
                .ReturnsAsync(() => new GetFrameworksQueryResponse
                {
                    Frameworks = new List<Framework>()
                });

            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetRelationshipByCommitmentQueryRequest>()))
                .ReturnsAsync(() => new GetRelationshipByCommitmentQueryResponse
                {
                    Relationship = new Relationship
                    {
                        Verified = true
                    }
                });

            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>()))
                .ReturnsAsync(() => new GetOverlappingApprenticeshipsQueryResponse
                {
                    Overlaps = new List<ApprenticeshipOverlapValidationResult>()
                });

            return mockMediator;
        }
    }
}
