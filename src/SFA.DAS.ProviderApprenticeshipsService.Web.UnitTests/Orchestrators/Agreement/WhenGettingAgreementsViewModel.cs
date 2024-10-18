using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.Agreement;

[TestFixture]
public class WhenGettingAgreementsViewModel
{
    private AgreementOrchestrator _orchestrator;
    private Mock<IMediator> _mediator;
    private Mock<IAgreementMapper> _agreementMapper;
    private const long ProviderId = 54321L;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _mediator.Setup(m => m.Send(It.IsAny<GetCommitmentAgreementsQueryRequest>(), new CancellationToken()))
            .ReturnsAsync(new GetCommitmentAgreementsQueryResponse
            {
                CommitmentAgreements = new List<ProviderCommitmentAgreement>
                {
                    new() { AccountLegalEntityPublicHashedId = "1" , LegalEntityName = "A"},
                    new() { AccountLegalEntityPublicHashedId = "2" , LegalEntityName = "B"},
                    new() { AccountLegalEntityPublicHashedId = "3" , LegalEntityName = "C"},
                    new() { AccountLegalEntityPublicHashedId = "4" , LegalEntityName = "D"},
                }
            });

        _agreementMapper = new Mock<IAgreementMapper>();

        _orchestrator = new AgreementOrchestrator(_mediator.Object,
            Mock.Of<IProviderCommitmentsLogger>(),
            _agreementMapper.Object);
    }

    [Test]
    public async Task TheCommitmentAgreementsReturnedFromHandlerAreMapped()
    {
        //Arrange
        _mediator.Setup(m => m.Send(It.IsAny<GetCommitmentAgreementsQueryRequest>(), new CancellationToken()))
            .ReturnsAsync(new GetCommitmentAgreementsQueryResponse
            {
                CommitmentAgreements = new List<ProviderCommitmentAgreement>
                {
                    new()
                }
            });

        var mappedCommitmentAgreement = new Web.Models.Agreement.CommitmentAgreement
        {
            AgreementID = "agree",
            OrganisationName = "org"
        };

        _agreementMapper.Setup(m => m.Map(It.IsAny<ProviderCommitmentAgreement>()))
            .Returns(mappedCommitmentAgreement);

        //Act
        var result = await _orchestrator.GetAgreementsViewModel(ProviderId, string.Empty);

        //Assert
        result.Should().NotBeNull();
        result.CommitmentAgreements.Should().AllBeEquivalentTo(mappedCommitmentAgreement);
    }

    [Test]
    public async Task TheCommitmentAgreementsReturnedFromHandlerAreMappedAndOrdered()
    {
        //Arrange
        SetOrganisations();

        //Act
        var result = await _orchestrator.GetAgreementsViewModel(ProviderId, string.Empty);

        //Assert
        result.Should().NotBeNull();
        result.CommitmentAgreements.Should().BeEquivalentTo(new object[]
        {
            new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "A", AgreementID = "1" },
            new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "B", AgreementID = "2" },
            new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "C", AgreementID = "3" },
            new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "D", AgreementID = "4" }
        });
    }

    [Test]
    public async Task TheCommitmentAgreementsReturnedFromHandlerAreMappedAndOrderedBySearchText()
    {
        //Arrange
        SetOrganisations();

        //Act
        var result = await _orchestrator.GetAgreementsViewModel(ProviderId, "A");

        //Assert
        result.Should().NotBeNull();
        result.CommitmentAgreements.Should().BeEquivalentTo(new object[] { new Web.Models.Agreement.CommitmentAgreement { OrganisationName = "A", AgreementID = "1"} });
    }

    [Test]
    public async Task TheCommitmentAgreementsReturnedFromHandlerAreMappedAndOrderedBySearchTextWithNoResults()
    {
        //Arrange
        SetOrganisations();

        //Act
        var result = await _orchestrator.GetAgreementsViewModel(ProviderId, "V");

        //Assert
        result.Should().NotBeNull();
        Assert.That(0, Is.EqualTo(result.CommitmentAgreements.Count()));
    }

    [Test]
    public async Task TheCommitmentAgreementsReturnedFromHandlerAreMappedAndOrderedBySearchTextWithoutDuplicateResults()
    {
        //Arrange
        SetOrganisations();

        //Act
        var result = await _orchestrator.GetAgreementsViewModel(ProviderId, "D");

        //Assert
        result.Should().NotBeNull();
        Assert.That(1, Is.EqualTo(result.CommitmentAgreements.Count()));
    }

    [Test]
    public async Task ThenAllOrganisationNamesAreReturnedWithFilteredResults()
    {
        //Arrange
        SetOrganisations();

        //Act
        var result = await _orchestrator.GetAgreementsViewModel(ProviderId, "D");

        //Assert
        result.Should().NotBeNull();
        Assert.That(1, Is.EqualTo(result.CommitmentAgreements.Count()));
        Assert.That(4, Is.EqualTo(result.AllProviderOrganisationNames.Count()));
    }

    private void SetOrganisations()
    {
        SetupMapping("1", "A");
        SetupMapping("2", "B");
        SetupMapping("3", "C");
        SetupMapping("4", "D");
        SetupMapping("4", "D");
    }

    private void SetupMapping(string publicHashedId, string outOrganisationName)
    {
        _agreementMapper
            .Setup(m => m.Map(It.Is<ProviderCommitmentAgreement>(ca => ca.AccountLegalEntityPublicHashedId == publicHashedId)))
            .Returns(new Web.Models.Agreement.CommitmentAgreement { OrganisationName = outOrganisationName, AgreementID = publicHashedId });
    }
}