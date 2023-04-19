using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetClientContent;

public class WhenIGetCachedContentBanner
{
    private const string CacheKey = "das-providerapprenticeshipsservice-web_banner";
    private IContentApiClient _sut;
    private Mock<IContentApiClient> _contentApiClientMock;
    private Mock<ICacheStorageService> _cacheStorageServiceMock;
    private string _contentType;
    private string _applicationId;
    public string ContentBanner;
    private static ProviderApprenticeshipsServiceConfiguration _providerApprenticeshipsServiceConfiguration;
    
    [SetUp]
    public void Arrange()
    {
        _providerApprenticeshipsServiceConfiguration = new ProviderApprenticeshipsServiceConfiguration()
        {
            ContentApplicationId = "das-providerapprenticeshipsservice-web",
            DefaultCacheExpirationInMinutes = 1
        };
            
        _contentType = "banner";
        _applicationId = "das-providerapprenticeshipsservice-web";
        _contentApiClientMock = new Mock<IContentApiClient>();
        _cacheStorageServiceMock = new Mock<ICacheStorageService>();
        _contentApiClientMock
            .Setup(mock => mock.Get(_contentType, _applicationId))
            .ReturnsAsync(ContentBanner);

        _sut = new ContentApiClientWithCaching(_contentApiClientMock.Object, _cacheStorageServiceMock.Object, _providerApprenticeshipsServiceConfiguration);
    }


    [Test]
    public async Task WhenBannerExistsInCache_ThenShouldReturnCachedBanner()
    {
        //Arrange
        var cachedContentBanner = "<p>From cache: find out how you can pause your apprenticeships<p>";

        _cacheStorageServiceMock
            .Setup(mock => mock.TryGet(CacheKey, out cachedContentBanner))
            .Returns(true);

        //Act
        var result = await _sut.Get(_contentType, _applicationId);

        //Assert
        Assert.That(cachedContentBanner, Is.EqualTo(result));
    }

    [Test]
    public async Task WhenBannerExistsInCache_ThenShouldNotCallApi()
    {
        //Arrange
        var cachedContentBanner = "<p>From cache: find out how you can pause your apprenticeships<p>";


        _cacheStorageServiceMock
            .Setup(mock => mock.TryGet(CacheKey, out cachedContentBanner))
            .Returns(true);

        //Act
        await _sut.Get(_contentType, _applicationId);

        //Assert
        _contentApiClientMock.Verify(x => x.Get(_contentType, _applicationId), Times.Never);
    }

    [Test]
    public async Task WhenBannerDoesNotExistInCache_ThenGetApiContent()
    {
        //Arrange

        //Act
        await _sut.Get(_contentType, _applicationId);

        //Assert
        _contentApiClientMock.Verify(x => x.Get(_contentType, _applicationId), Times.Once);
    }

    [Test]
    public async Task WhenBannerDoesNotExistInCache_ThenCacheApiContent()
    {
        //Arrange
        const string apiContentBanner = "<p>From API: find out how you can pause your apprenticeships<p>";

        _contentApiClientMock
            .Setup(mock => mock.Get(_contentType, _applicationId))
            .ReturnsAsync(apiContentBanner);

        //Act
        await _sut.Get(_contentType, _applicationId);

        //Assert
        _cacheStorageServiceMock.Verify(x => x.Save(CacheKey, apiContentBanner, _providerApprenticeshipsServiceConfiguration.DefaultCacheExpirationInMinutes), Times.Once);
    }

    [Test]
    public async Task WhenBannerDoesNotExistInCache_ThenReturnApiContent()
    {
        //Arrange
        const string apiContentBanner = "<p>From API: find out how you can pause your apprenticeships<p>";

        _contentApiClientMock
            .Setup(mock => mock.Get(_contentType, _applicationId))
            .ReturnsAsync(apiContentBanner);

        //Act
        var result = await _sut.Get(_contentType, _applicationId);

        //Assert
        Assert.That(apiContentBanner, Is.EqualTo(result));
    }
}