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
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetRelationshipByCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;
using CommitmentView = SFA.DAS.Commitments.Api.Types.Commitment.CommitmentView;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    using Task = System.Threading.Tasks.Task;

    public class WhenGettingCommitmentViewModel : ApprenticeshipValidationTestBase
    {
        [Test(Description = "Should return false on PendingChanges if overall agreement status is EmployerAgreed")]
        public async Task ShouldCommitmentWithEmployerAndBothAgreed()
        {
            var commitment = new CommitmentView
            {
                AgreementStatus = AgreementStatus.EmployerAgreed,
                EditStatus = EditStatus.ProviderOnly,
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

            result.PendingChanges.ShouldBeEquivalentTo(false);
        }

        [Test(Description = "Should return true on PendingChanges overall agreement status isn't EmployerAgreed")]
        public async Task CommitmentWithOneProviderAgreed()
        {
            var commitment = new CommitmentView
            {
                AgreementStatus = AgreementStatus.ProviderAgreed,
                EditStatus = EditStatus.ProviderOnly,
                Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship (),
                    new Apprenticeship ()
                },
                Messages = new List<MessageView>()
            };

            _mockMediator = GetMediator(commitment);
            SetUpOrchestrator();
            var result = await _orchestrator.GetCommitmentDetails(1L, "ABBA213");

            result.PendingChanges.ShouldBeEquivalentTo(true);
        }

        [TestCase(EditStatus.EmployerOnly, true)]
        [TestCase(EditStatus.ProviderOnly, false)]
        [TestCase(EditStatus.Neither, true)]
        public async Task ThenCommitmentReadOnlyFlagIsSet(EditStatus editStatus, bool expectedIsReadOnly)
        {
            var commitment = new CommitmentView
            {
                AgreementStatus = AgreementStatus.ProviderAgreed,
                EditStatus = editStatus,
                Apprenticeships = new List<Apprenticeship>(),
                Messages = new List<MessageView>()
            };

            _mockMediator = GetMediator(commitment);
            SetUpOrchestrator();
            var result = await _orchestrator.GetCommitmentDetails(1L, "ABBA213");

            result.IsReadOnly.ShouldBeEquivalentTo(expectedIsReadOnly);
        }

        [Test]
        public async Task ThenFrameworksAreNotRetrievedForCohortsFundedByTransfer()
        {
            var commitment = new CommitmentView
            {
                AgreementStatus = AgreementStatus.ProviderAgreed,
                EditStatus = EditStatus.ProviderOnly,
                Apprenticeships = new List<Apprenticeship>(),
                Messages = new List<MessageView>(),
                TransferSender = new TransferSender {Id = 99, Name = "Transfer Sender Org", TransferApprovalStatus = TransferApprovalStatus.Pending}
            };

            _mockMediator = GetMediator(commitment);
            SetUpOrchestrator();
            await _orchestrator.GetCommitmentDetails(1L, "ABBA213");

            _mockMediator.Verify(x => x.SendAsync(It.Is<GetTrainingProgrammesQueryRequest>(r => !r.IncludeFrameworks)), Times.Once);
        }

        [Test]
        public async Task ThenFrameworksAreRetrievedForCohortsNotFundedByTransfer()
        {
            var commitment = new CommitmentView
            {
                AgreementStatus = AgreementStatus.ProviderAgreed,
                EditStatus = EditStatus.ProviderOnly,
                Apprenticeships = new List<Apprenticeship>(),
                Messages = new List<MessageView>(),
                TransferSender = null
            };

            _mockMediator = GetMediator(commitment);
            SetUpOrchestrator();
            await _orchestrator.GetCommitmentDetails(1L, "ABBA213");

            _mockMediator.Verify(x => x.SendAsync(It.Is<GetTrainingProgrammesQueryRequest>(r => r.IncludeFrameworks)), Times.Once);
        }

        [TestCase(123L, true)]
        [TestCase(null, false)]
        public async Task ThenTheCommitmentIsMarkedAsFundedByTransferIfItHasATransferSenderId(long? transferSenderId, bool expectedTransferFlag)
        {
            var commitment = new CommitmentView
            {
                AgreementStatus = AgreementStatus.ProviderAgreed,
                EditStatus = EditStatus.ProviderOnly,
                Apprenticeships = new List<Apprenticeship>(),
                Messages = new List<MessageView>(),
                TransferSender = (transferSenderId != null ? new TransferSender { Id = transferSenderId, TransferApprovalStatus = TransferApprovalStatus.Pending } : null)
            };

            _mockMediator = GetMediator(commitment);
            SetUpOrchestrator();
            var result = await _orchestrator.GetCommitmentDetails(1L, "ABBA213");

            Assert.AreEqual(expectedTransferFlag, result.IsFundedByTransfer);
        }

        // --- Helpers ---

        protected static Mock<IMediator> GetMediator(CommitmentView commitment)
        {
            var respons = new GetCommitmentQueryResponse
            {
                Commitment = commitment
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .Returns(Task.Factory.StartNew(() => respons));

            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetTrainingProgrammesQueryRequest>()))
                .ReturnsAsync(new GetTrainingProgrammesQueryResponse { TrainingProgrammes = new List<ITrainingProgramme>() });

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
