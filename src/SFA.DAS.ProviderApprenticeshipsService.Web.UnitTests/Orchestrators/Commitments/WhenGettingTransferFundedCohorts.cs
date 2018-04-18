using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Commitments
{
    [TestFixture]
    public class WhenGettingTransferFundedCohorts : ApprenticeshipValidationTestBase
    {

        private GetCommitmentsQueryResponse _response;
        [SetUp]
        public override void SetUp()
        {
            _response = new GetCommitmentsQueryResponse
            {
                Commitments = new List<CommitmentListItem>
                {
                    new CommitmentListItem
                    {
                        Id = 1,
                        LegalEntityName = "LEName",
                        EmployerAccountId = 11,
                        TransferApprovalStatus = TransferApprovalStatus.Pending,
                        TransferSenderId = 123,
                        TransferSenderName = "Name",
                        EditStatus = EditStatus.Both
                    },
                    new CommitmentListItem
                    {
                        Id = 2,
                        LegalEntityName = "LEName2",
                        EmployerAccountId = 21,
                        TransferApprovalStatus = TransferApprovalStatus.Rejected,
                        TransferSenderId = 123,
                        TransferSenderName = "Name",
                        EditStatus = EditStatus.EmployerOnly

                    },
                    new CommitmentListItem
                    {
                        Id = 3,
                        LegalEntityName = "LEName3",
                        EmployerAccountId = 31,
                        TransferApprovalStatus = TransferApprovalStatus.Approved,
                        TransferSenderId = 123,
                        TransferSenderName = "Name",
                        EditStatus = EditStatus.Both
                    },
                    new CommitmentListItem
                    {
                        Id = 4,
                        LegalEntityName = "LEName4",
                        EmployerAccountId = 41,
                        EditStatus = EditStatus.Neither
                    }

                }
            };
            _mockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentsQueryRequest>())).ReturnsAsync(_response);
            _mockHashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns((long p) => $"RST{p}");

            base.SetUp();
        }

        [Test]
        public async Task ShouldPassProviderIdToRequest()
        {
            await _orchestrator.GetAllTransferFunded(12222);

            _mockMediator.Verify(x=>x.SendAsync(It.Is<GetCommitmentsQueryRequest>(p=>p.ProviderId == 12222)));
        }


        [Test]
        public async Task ShouldSetProviderIdAndFilterResultsToFirstTwoItems()
        {
            var result = await _orchestrator.GetAllTransferFunded(12222);

            result.ProviderId.Should().Be(12222);
            result.Commitments.Count().Should().Be(2);
            var list = result.Commitments.ToList();
            list[0].HashedCommitmentId.Should().Be("RST1");
            list[1].HashedCommitmentId.Should().Be("RST2");
        }


        [Test]
        public async Task ShouldMapCommitmentItemsToTransferViewModel()
        {
            var result = await _orchestrator.GetAllTransferFunded(12222);

            var list = result.Commitments.ToList();
            list[0].ReceivingEmployerName.Should().Be("LEName");
            list[0].Status.Should().Be(TransferApprovalStatus.Pending);
            list[1].ReceivingEmployerName.Should().Be("LEName2");
            list[1].Status.Should().Be(TransferApprovalStatus.Rejected);
        }
    }
}
