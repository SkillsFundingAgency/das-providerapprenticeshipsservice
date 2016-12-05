using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class WhenApproveCommitment
    {
        [TestCase(SaveStatus.ApproveAndSend, LastAction.Approve, true)]
        [TestCase(SaveStatus.AmendAndSend, LastAction.Amend, true)]
        [TestCase(SaveStatus.Approve, LastAction.Approve, false)]
        public async Task  CheckStatusUpdate(SaveStatus input, LastAction expectedLastAction, bool expectedCreateTaskBool)
        {
            var mockMediator = new Mock<IMediator>();
            var mockHashingService = new Mock<IHashingService>();
            mockHashingService.Setup(m => m.DecodeValue("ABBA99")).Returns(2L);

            var _sut = new CommitmentOrchestrator(mockMediator.Object, Mock.Of<ICommitmentStatusCalculator>(), mockHashingService.Object);
            await _sut.SubmitCommitment(1L, "ABBA99", input, string.Empty);

            mockMediator.Verify(m => m
                .SendAsync(It.Is<SubmitCommitmentCommand>(
                    p => p.ProviderId == 1L &&
                    p.CommitmentId == 2L &&
                    p.Message == string.Empty &&
                    p.LastAction == expectedLastAction &&
                    p.CreateTask == expectedCreateTaskBool )));
        }

        [Test]
        public async Task SubmitCommitemtWithSaveShouldDoNothing()
        {
            var mockMediator = new Mock<IMediator>();

            var _sut = new CommitmentOrchestrator(mockMediator.Object, Mock.Of<ICommitmentStatusCalculator>(), Mock.Of<IHashingService>());
            await _sut.SubmitCommitment(1L, "ABBA12", SaveStatus.Save, "");

            mockMediator.Verify(m => m
                .SendAsync(It.IsAny<SubmitCommitmentCommand>()), Times.Never);
        }
    }
}
