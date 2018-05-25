using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenGettingAcknowledgementViewModel: ApprenticeshipValidationTestBase
    {
        private CommitmentView _commitment;

        public override void SetUp()
        {
            _commitment = new CommitmentView
            {
                Messages = new List<MessageView>()
            };

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(() => new GetCommitmentQueryResponse
                {
                    Commitment = _commitment
                });

            base.SetUp();
        }

        [Test]
        public async Task ThenARequestForTheCommitmentDataIsMade()
        {
            await _orchestrator.GetAcknowledgementViewModel(1, "Hashed-Id", Web.Models.Types.SaveStatus.ApproveAndSend);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()),
                Times.Once);
        }
    }
}
