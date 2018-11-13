using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using CommitmentView = SFA.DAS.Commitments.Api.Types.Commitment.CommitmentView;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenApproveCommitment : ApprenticeshipValidationTestBase
    {
        [TestCase(SaveStatus.ApproveAndSend, LastAction.Approve, true)]
        [TestCase(SaveStatus.AmendAndSend, LastAction.Amend, true)]
        [TestCase(SaveStatus.Approve, LastAction.Approve, false)]
        [TestCase(SaveStatus.Save, LastAction.None, false)]
        public async Task CheckStatusUpdate(SaveStatus input, LastAction expectedLastAction, bool expectedCreateTaskBool)
        {
            _mockHashingService.Setup(m => m.DecodeValue("ABBA99")).Returns(2L);
            _mockMediator.Setup(m => m.Send(It.IsAny<GetProviderAgreementQueryRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new GetProviderAgreementQueryResponse { HasAgreement = ProviderAgreementStatus.Agreed }));

            _mockMediator.Setup(m => m.Send(It.IsAny<GetCommitmentQueryRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        AgreementStatus = AgreementStatus.NotAgreed,
                        EditStatus = EditStatus.ProviderOnly
                    }
                }));

            await _orchestrator.SubmitCommitment("UserId", 1L, "ABBA99", input, string.Empty, new SignInUserModel());

           _mockMediator.Verify(m => m
                .Send(It.Is<SubmitCommitmentCommand>(
                    p => p.ProviderId == 1L &&
                    p.CommitmentId == 2L &&
                    p.Message == string.Empty &&
                    p.LastAction == expectedLastAction &&
                    p.CreateTask == expectedCreateTaskBool), It.IsAny<CancellationToken>()));
        }
    }
}
