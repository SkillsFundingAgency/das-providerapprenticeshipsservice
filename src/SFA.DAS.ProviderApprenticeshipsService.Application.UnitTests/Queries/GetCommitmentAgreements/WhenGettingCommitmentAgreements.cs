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

            _handler = new GetCommitmentAgreementsQueryHandler(_commitmentsApi.Object);
        }

        [Test]
        public async Task ThenCommitmentAgreementsAreReturned()
        {
            const long providerId = 777L;

            var query = new GetCommitmentAgreementsQueryRequest { ProviderId = providerId };

            var commitmentAgreements = new List<CommitmentAgreement>
            {
                new CommitmentAgreement {Reference = "R1", AccountLegalEntityPublicHashedId = "A1", LegalEntityName = "L1"},
                new CommitmentAgreement {Reference = "R2", AccountLegalEntityPublicHashedId = "A2", LegalEntityName = "L2"},
            };

            _commitmentsApi.Setup(x => x.GetCommitmentAgreements(providerId))
                .ReturnsAsync(() => TestHelper.Clone(commitmentAgreements));

            var response = await _handler.Handle(query);

            TestHelper.EnumerablesAreEqual(commitmentAgreements, response.CommitmentAgreements);
        }
    }
}
