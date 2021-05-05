using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Domain;

using CommitmentView = SFA.DAS.Commitments.Api.Types.Commitment.CommitmentView;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenFinishEditing : ApprenticeshipValidationTestBase
    {
        private CommitmentView _testCommitment;

        [Test]
        public override void SetUp()
        {
            _testCommitment = new CommitmentView
            {
                EditStatus = EditStatus.ProviderOnly,
                ProviderId = 1L,
                Apprenticeships = new List<Apprenticeship>
                {
                }
            };

            base.SetUp();
        }

        [TestCase(false, Description = "Should return NOT ReadyForApproval if the Cohort Is NOT Complete For Provider")]
        [TestCase(false, Description = "Should return ReadyForApproval if the Cohort Is Complete For Provider")]
        public void ShouldReturnExpectedReadyForApprovalStateAccordingToCohortState(bool isCompleteForProvider)
        {
            _testCommitment.Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship { AgreementStatus = AgreementStatus.ProviderAgreed, CanBeApproved = false },
                    new Apprenticeship { AgreementStatus = AgreementStatus.ProviderAgreed, CanBeApproved = true }
                };

            _mockMediator = GetMediator(_testCommitment);
            _mockMediator.Setup(m => m.Send(It.IsAny<GetProviderAgreementQueryRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new GetProviderAgreementQueryResponse { HasAgreement = ProviderAgreementStatus.Agreed }));

            _mockCommitmentsV2Service.Setup(x => x.CohortIsCompleteForProvider(It.IsAny<long>()))
                .ReturnsAsync(isCompleteForProvider);
            SetUpOrchestrator();
            var result = _orchestrator.GetFinishEditing(1L, "ABBA123").Result;

            result.ReadyForApproval.Should().Be(isCompleteForProvider);
        }

        [Test(Description = "Should return ApproveAndSend if at least one apprenticeship is ProviderAgreed")]
        public void CommitmentWithOneProviderAgreed()
        {
            _testCommitment.CanBeApproved = true;
            _testCommitment.Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship { AgreementStatus = AgreementStatus.BothAgreed },
                    new Apprenticeship { AgreementStatus = AgreementStatus.EmployerAgreed },
                    new Apprenticeship { AgreementStatus = AgreementStatus.ProviderAgreed }
                };

            _mockMediator = GetMediator(_testCommitment);
            SetUpOrchestrator();
            var result = _orchestrator.GetFinishEditing(1L, "ABBA123").Result;

            result.IsApproveAndSend.Should().BeTrue();
        }

        [Test(Description = "Should return ApproveAndSend if at least one apprenticeship is NotAgreed")]
        public void CommitmentWithOneNotAgreed()
        {
            _testCommitment.CanBeApproved = true;
            _testCommitment.Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship { AgreementStatus = AgreementStatus.BothAgreed },
                    new Apprenticeship { AgreementStatus = AgreementStatus.EmployerAgreed },
                    new Apprenticeship { AgreementStatus = AgreementStatus.NotAgreed }
                };

            _mockMediator = GetMediator(_testCommitment);
            SetUpOrchestrator();
            var result = _orchestrator.GetFinishEditing(1L, "ABBA123").Result;

            result.IsApproveAndSend.Should().BeTrue();
        }

        [Test(Description = "Should return ApproveOnly all are Employer Agreed")]
        public void CommitmentAllEmployerAgreed()
        {
            _testCommitment.CanBeApproved = true;
            _testCommitment.Apprenticeships = new List<Apprenticeship>
                {
                    new Apprenticeship { AgreementStatus = AgreementStatus.EmployerAgreed },
                    new Apprenticeship { AgreementStatus = AgreementStatus.EmployerAgreed },
                    new Apprenticeship { AgreementStatus = AgreementStatus.EmployerAgreed }
                };

            _mockMediator = GetMediator(_testCommitment);
            SetUpOrchestrator();
            var result = _orchestrator.GetFinishEditing(1L, "ABBA123").Result;

            result.IsApproveAndSend.Should().BeFalse();
        }

        // --- Helpers ---

        private Mock<IMediator> GetMediator(CommitmentView commitment)
        {
            var respons = new GetCommitmentQueryResponse
            {
                Commitment = commitment
            };

            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.Send(It.IsAny<GetCommitmentQueryRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Factory.StartNew(() => respons));

            mockMediator.Setup(m => m.Send(It.IsAny<GetProviderAgreementQueryRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new GetProviderAgreementQueryResponse { HasAgreement = ProviderAgreementStatus.Agreed }));

            mockMediator.Setup(m => m.Send(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetOverlappingApprenticeshipsQueryResponse
                {
                    Overlaps = new List<ApprenticeshipOverlapValidationResult>()
                });

            return mockMediator;
        }
    }
}
