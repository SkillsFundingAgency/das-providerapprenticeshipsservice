using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetCommitmentAgreements;

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

    [Test, AutoData]
    public async Task ThenCommitmentAgreementsAreReturned(long providerId)
    {
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
            .ReturnsAsync(expectedResponse);

        var response = await _handler.Handle(query, new CancellationToken());

        expectedResponse.ProviderCommitmentAgreement.Should().BeEquivalentTo(response.CommitmentAgreements);
    }
}