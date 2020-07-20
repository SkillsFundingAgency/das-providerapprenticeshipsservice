using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetClientContent;
using Moq;
using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using System.Threading;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetClientContent
{
    public class WhenIGetContentBanner
    {
        private GetClientContentRequestHandler _handler;
        private GetClientContentRequest _request;        
        public Mock<ICacheStorageService> MockCacheStorageService;
        private Mock<IClientContentService> _contentBannerService;
        private string _contentType;
        private string _clientId;
        private Mock<ILog> _logger;
        public string ContentBanner;
        public static ProviderApprenticeshipsServiceConfiguration ProviderApprenticeshipsServiceConfiguration;

        [SetUp]
        public void Arrange()
        {
            ProviderApprenticeshipsServiceConfiguration = new ProviderApprenticeshipsServiceConfiguration()
            {
                ContentApplicationId = "das-providerapprenticeshipsservice-web",
                DefaultCacheExpirationInMinutes = 1
            };
            ContentBanner = "<p>find out how you can pause your apprenticeships<p>";
            MockCacheStorageService = new Mock<ICacheStorageService>();
            _contentType = "banner";
            _clientId = "das-providerapprenticeshipsservice-web";
            _logger = new Mock<ILog>();
            _contentBannerService = new Mock<IClientContentService>();
            _contentBannerService
                .Setup(cbs => cbs.Get(_contentType, _clientId))
                .ReturnsAsync(ContentBanner);

            _request = new GetClientContentRequest
            {
                ContentType = "banner"
            };

            _handler = new GetClientContentRequestHandler( _logger.Object,
                _contentBannerService.Object, MockCacheStorageService.Object, ProviderApprenticeshipsServiceConfiguration);
        }


        [Test]
        public async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await _handler.Handle(_request, new CancellationToken());

            //Assert
            _contentBannerService.Verify(x => x.Get(_contentType, _clientId), Times.Once);
        }

        [Test]
        public async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await _handler.Handle(_request, new CancellationToken());

            //Assert
            Assert.AreEqual(ContentBanner, response.Content);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Check_Cache_ReturnIfExists(GetClientContentRequest query, 
            string contentBanner,
            Mock<ICacheStorageService> cacheStorageService,
            GetClientContentRequestHandler requestHandler,            
            Mock<ILog> logger,
            Mock<IClientContentService> clientMockontentService)
        {
            //Arrange            
            query.ContentType = "Banner";
            query.UseLegacyStyles = false;

            var key = ProviderApprenticeshipsServiceConfiguration.ContentApplicationId;

            clientMockontentService.Setup(c => c.Get("banner", key));

            requestHandler = new GetClientContentRequestHandler(logger.Object, clientMockontentService.Object,
                cacheStorageService.Object, ProviderApprenticeshipsServiceConfiguration);

            var cacheKey = key + "_banner";
            cacheStorageService.Setup(c => c.TryGet(cacheKey, out contentBanner))
                .Returns(true);
            
            //Act
            var result = await requestHandler.Handle(query, new CancellationToken());

            //assert
            Assert.AreEqual(result.Content, contentBanner);
            cacheStorageService.Verify(x => x.TryGet(cacheKey, out contentBanner), Times.Once);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Check_Cache_ReturnNull_CallFromClient(GetClientContentRequest query, 
            string contentBanner,
            Mock<ICacheStorageService> cacheStorageService,
            GetClientContentRequestHandler requestHandler,            
            Mock<ILog> logger,
            Mock<IClientContentService> clientMockContentService)
        {
            //Arrange
            var key = ProviderApprenticeshipsServiceConfiguration.ContentApplicationId;
            query.ContentType = "Banner";
            query.UseLegacyStyles = false;

            string nullCacheString = null;
            var cacheKey = key + "_banner";
            cacheStorageService.Setup(c => c.TryGet(cacheKey, out nullCacheString))
                .Returns(false);

            clientMockContentService.Setup(c => c.Get(query.ContentType, key))
                .ReturnsAsync(contentBanner);

            requestHandler = new GetClientContentRequestHandler(logger.Object, clientMockContentService.Object,
                cacheStorageService.Object, ProviderApprenticeshipsServiceConfiguration);

            //Act
            var result = await requestHandler.Handle(query, new CancellationToken());

            //assert
            Assert.AreEqual(result.Content, contentBanner);
        }

    }
}
