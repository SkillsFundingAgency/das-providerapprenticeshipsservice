using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using CommitmentView = SFA.DAS.Commitments.Api.Types.Commitment.CommitmentView;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenApproveCommitment
    {
        [TestCase(SaveStatus.ApproveAndSend, LastAction.Approve, true)]
        [TestCase(SaveStatus.AmendAndSend, LastAction.Amend, true)]
        [TestCase(SaveStatus.Approve, LastAction.Approve, false)]
        [TestCase(SaveStatus.Save, LastAction.None, false)]
        public async Task CheckStatusUpdate(SaveStatus input, LastAction expectedLastAction, bool expectedCreateTaskBool)
        {
            var mockMediator = new Mock<IMediator>();
            var mockHashingService = new Mock<IHashingService>();
            mockHashingService.Setup(m => m.DecodeValue("ABBA99")).Returns(2L);
            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetProviderAgreementQueryRequest>()))
                .Returns(Task.FromResult(new GetProviderAgreementQueryResponse { HasAgreement = ProviderAgreementStatus.Agreed }));

            mockMediator.Setup(m => m.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .Returns(Task.FromResult(new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        AgreementStatus = AgreementStatus.NotAgreed,
                        EditStatus = EditStatus.ProviderOnly
                    }
                }));

            var _sut = new CommitmentOrchestrator(mockMediator.Object, Mock.Of<ICommitmentStatusCalculator>(), mockHashingService.Object, Mock.Of<IProviderCommitmentsLogger>(), Mock.Of<ApprenticeshipViewModelUniqueUlnValidator>(), Mock.Of<ProviderApprenticeshipsServiceConfiguration>(), Mock.Of<IApprenticeshipMapper>());
            await _sut.SubmitCommitment("UserId", 1L, "ABBA99", input, string.Empty, new SignInUserModel());

            mockMediator.Verify(m => m
                .SendAsync(It.Is<SubmitCommitmentCommand>(
                    p => p.ProviderId == 1L &&
                    p.CommitmentId == 2L &&
                    p.Message == string.Empty &&
                    p.LastAction == expectedLastAction &&
                    p.CreateTask == expectedCreateTaskBool )));
        }
    }
}
