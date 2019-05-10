using System.Threading;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment;
using SFA.DAS.Commitments.Api.Types.Commitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    using Task = System.Threading.Tasks.Task;

    public class WhenGettingLegalEntityPublicHashedIdFromCommitmentDetails : ApprenticeshipValidationTestBase
    {
        [Test]
        public async Task ShouldReturnIdInPublicHashedFormat()
        {
            var commitment = new CommitmentView
            {
                EmployerAccountId = 1L
            };

            _mockMediator.Setup(x => x.Send(It.IsAny<GetCommitmentQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCommitmentQueryResponse { Commitment = commitment});

            var result = await _orchestrator.GetEmployerAccountLegalEntityPublicHashedIdFromCommitment(1L, "ABBA123");

            result.Should().Be("X1X");
        }
    }
}