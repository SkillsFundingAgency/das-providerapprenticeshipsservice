namespace SFA.DAS.ProviderApprenticeshipsService.Web.Tests.Orchestrators
{
    using System.Threading.Tasks;

    using MediatR;

    using Moq;

    using NUnit.Framework;

    using Application.Commands.SubmitCommitment;
    using Models.Types;

    using SFA.DAS.Commitments.Api.Types;

    using Web.Orchestrators;

    [TestFixture]
    public class WhenApproveCommitment
    {
        [TestCase(SaveStatus.Save, AgreementStatus.NotAgreed, false)]
        public async Task  Bla(SaveStatus input, AgreementStatus test1, bool test2)
        {
            var s = new SubmitCommitmentCommand
                        {
                ProviderId = 1L,
                CommitmentId = 2L,
                Message = string.Empty,
                AgreementStatus = AgreementStatus.BothAgreed,
                CreateTask = false
            };
            var mockMediator = new Mock<IMediator>();

            var _sut = new CommitmentOrchestrator(mockMediator.Object, Mock.Of<ICommitmentStatusCalculator>());
            await _sut.ApproveCommitment(1L, 2L, SaveStatus.Save);
            mockMediator.Verify(m => m
                .SendAsync(It.Is<SubmitCommitmentCommand>(
                    p => p.ProviderId == 1L &&
                    p.CommitmentId == 2L &&
                    p.Message == string.Empty &&
                    p.AgreementStatus == AgreementStatus.NotAgreed &&
                    p.CreateTask == false )));
            
            //ApproveCommitment
        }
    }
}
