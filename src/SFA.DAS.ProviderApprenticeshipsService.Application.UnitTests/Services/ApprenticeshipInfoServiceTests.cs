using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Services;

[TestFixture]
public class ApprenticeshipInfoServiceTests
{
    private ApprenticeshipInfoService _sut;

    private Mock<ICommitmentsV2ApiClient> _commitmentsV2ApiClient;

    [SetUp]
    public void Arrange()
    {
        _commitmentsV2ApiClient = new Mock<ICommitmentsV2ApiClient>();

        _sut = new ApprenticeshipInfoService(_commitmentsV2ApiClient.Object);
    }

    [Test]
    public async Task ReturnsProvidersView()
    {
        var autoFixture = new Fixture();
        var response = autoFixture.Create<GetProviderResponse>();

        _commitmentsV2ApiClient
            .Setup(x => x.GetProvider(response.ProviderId))
            .Returns(Task.FromResult(response));

        var result = await _sut.GetProvider(response.ProviderId);

        _commitmentsV2ApiClient.Verify(x => x.GetProvider(response.ProviderId));

        Assert.Multiple(() =>
        {
            Assert.That(result.Provider.ProviderName, Is.EqualTo(response.Name));
            Assert.That(result.Provider.Ukprn, Is.EqualTo(response.ProviderId));
        });
    }

    [Test]
    public async Task WhenExceptionIsThrownReturnsNull()
    {
        var autoFixture = new Fixture();
        var response = autoFixture.Create<GetProviderResponse>();

        _commitmentsV2ApiClient
            .Setup(x => x.GetProvider(response.ProviderId))
            .Throws(new Exception());

        var result = await _sut.GetProvider(response.ProviderId);

        _commitmentsV2ApiClient.Verify(x => x.GetProvider(response.ProviderId));
        Assert.That(result, Is.Null);
    }
}