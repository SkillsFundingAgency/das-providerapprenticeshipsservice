using Moq;
using NUnit.Framework;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetClientContent;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetClientContent
{
    public class WhenIGetCachedContentBanner
    {
        private const string CacheKey = "das-providerapprenticeshipsservice-web_banner";
        private IContentApiClient _sut;
        private GetClientContentRequest _request;
        private Mock<IContentApiClient> _contentApiClientMock;
        private Mock<ICacheStorageService> _cacheStorageServiceMock;
        private string _contentType;
        private string _applicationId;
        private Mock<ILog> _logger;
        public string ContentBanner;
        public static ProviderApprenticeshipsServiceConfiguration ProviderApprenticeshipsServiceConfiguration;
        private delegate void ServiceProcessValue(string inputValue, out string outputValue);

        [SetUp]
        public void Arrange()
        {
            ProviderApprenticeshipsServiceConfiguration = new ProviderApprenticeshipsServiceConfiguration()
            {
                ContentApplicationId = "das-providerapprenticeshipsservice-web",
                DefaultCacheExpirationInMinutes = 1
            };
            
            _contentType = "banner";
            _applicationId = "das-providerapprenticeshipsservice-web";
            _logger = new Mock<ILog>();
            _contentApiClientMock = new Mock<IContentApiClient>();
            _cacheStorageServiceMock = new Mock<ICacheStorageService>();
            _contentApiClientMock
                .Setup(mock => mock.Get(_contentType, _applicationId))
                .ReturnsAsync(ContentBanner);

            _request = new GetClientContentRequest
            {
                ContentType = "banner"
            };

            _sut = new ContentApiClientWithCaching(_contentApiClientMock.Object, _cacheStorageServiceMock.Object, ProviderApprenticeshipsServiceConfiguration);
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
            Assert.AreEqual(cachedContentBanner, result);
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
            string apiContentBanner = "<p>From API: find out how you can pause your apprenticeships<p>";

            _contentApiClientMock
                .Setup(mock => mock.Get(_contentType, _applicationId))
                .ReturnsAsync(apiContentBanner);

            //Act
            await _sut.Get(_contentType, _applicationId);

            //Assert
            _cacheStorageServiceMock.Verify(x => x.Save(CacheKey, apiContentBanner, ProviderApprenticeshipsServiceConfiguration.DefaultCacheExpirationInMinutes), Times.Once);
        }

        [Test]
        public async Task WhenBannerDoesNotExistInCache_ThenReturnApiContent()
        {
            //Arrange
            string apiContentBanner = "<p>From API: find out how you can pause your apprenticeships<p>";

            _contentApiClientMock
                .Setup(mock => mock.Get(_contentType, _applicationId))
                .ReturnsAsync(apiContentBanner);

            //Act
            var result = await _sut.Get(_contentType, _applicationId);

            //Assert
            Assert.AreEqual(apiContentBanner, result);
        }
    }
}
