using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetCommitmentAgreements
{
    [TestFixture]
    public class WhenGettingCommitmentAgreements
    {
        private Mock<ICommitmentsV2ApiClient> _commitmentsApi;
        private GetCommitmentAgreementsQueryHandler _handler;

        [SetUp]
        public void WhenGettingCommitmentAgreementsSetup()
        {
            _commitmentsApi = new Mock<ICommitmentsV2ApiClient>();

            _handler = new GetCommitmentAgreementsQueryHandler(_commitmentsApi.Object);
        }

        [Test]
        public async Task ThenCommitmentAgreementsAreReturned()
        {
            const long providerId = 777L;

            var query = new GetCommitmentAgreementsQueryRequest { ProviderId = providerId };

            var expectedResponse = new GetProviderCommitmentAgreementResponse
            {
                ProviderCommitmentAgreement = new List<ProviderCommitmentAgreement>
                {
                    new ProviderCommitmentAgreement {AccountLegalEntityPublicHashedId = "A1", LegalEntityName = "L1"},
                    new ProviderCommitmentAgreement {AccountLegalEntityPublicHashedId = "A2", LegalEntityName = "L2"},
                }
            };

            _commitmentsApi
                .Setup(x => x.GetProviderCommitmentAgreement(providerId))
                .ReturnsAsync(() => TestHelper.Clone(expectedResponse));

            var response = await _handler.Handle(query, new CancellationToken());

            TestHelper.EnumerablesAreEqual(expectedResponse.ProviderCommitmentAgreement, response.CommitmentAgreements);
        }
    }
}
