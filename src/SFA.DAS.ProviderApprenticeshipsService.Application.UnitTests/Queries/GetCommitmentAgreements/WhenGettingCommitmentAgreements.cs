using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetCommitmentAgreements
{
    [TestFixture]
    public class WhenGettingCommitmentAgreements
    {
        private Mock<IProviderCommitmentsApi> _commitmentsApi;
        private GetCommitmentAgreementsQueryHandler _handler;

        [SetUp]
        public void WhenGettingCommitmentAgreementsSetup()
        {
            _commitmentsApi = new Mock<IProviderCommitmentsApi>();
            _commitmentsApi.Setup(x => x.GetCommitmentAgreements(It.IsAny<long>()))
                .ReturnsAsync(() => new List<CommitmentAgreement>());

            _handler = new GetCommitmentAgreementsQueryHandler(_commitmentsApi.Object);
        }

        [Test]
        public async Task ThenCommitmentAgreementsAreReturned()
        {
            var query = new GetCommitmentAgreementsQueryRequest { ProviderId = 1 };

            var response = await _handler.Handle(query);
        }
    }
}
