using System.Threading;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    using Task = System.Threading.Tasks.Task;

    public class WhenGettingHashedIdsFromCommitmentDetails : ApprenticeshipValidationTestBase
    {
        [Test]
        public async Task ShouldReturnLegalEntityIdInPublicHashedFormatButNoTransferId()
        {
            var commitment = new CommitmentView
            {
                EmployerAccountId = 1L,
                AccountLegalEntityPublicHashedId = "X1X"
            };

            _mockMediator.Setup(x => x.Send(It.IsAny<GetCommitmentQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCommitmentQueryResponse { Commitment = commitment});

            var result = await _orchestrator.GetHashedIdsFromCommitment(1L, "ABBA123");

            result.HashedLegalEntityId.Should().Be("X1X");
            result.HashedTransferSenderId.Should().BeNull();
        }
        [Test]
        public async Task ShouldReturnBothLegalEntityIdAndTransferIdInPublicHashedFormat()
        {
            var commitment = new CommitmentView
            {
                EmployerAccountId = 1L,
                AccountLegalEntityPublicHashedId = "X1X",
                TransferSender = new TransferSender {  Id = 2L }
            };

            _mockMediator.Setup(x => x.Send(It.IsAny<GetCommitmentQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCommitmentQueryResponse { Commitment = commitment });

            var result = await _orchestrator.GetHashedIdsFromCommitment(1L, "ABBA123");

            result.HashedLegalEntityId.Should().Be("X1X");
            result.HashedTransferSenderId.Should().Be("X2X");
        }
    }
}