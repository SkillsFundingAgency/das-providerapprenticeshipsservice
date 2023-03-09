using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetClientContent;
using SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Extensions.LoggingMockExtensions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Queries.GetClientContent
{
    public class WhenIGetContentBanner
    {
        private GetClientContentRequestHandler _handler;
        private GetClientContentRequest _request;
        private Mock<IContentApiClient> _contentApiClientMock;
        private string _contentType;
        private string _clientId;
        private Mock<ILogger<GetClientContentRequestHandler>> _logger;
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
            _contentType = "banner";
            _clientId = "das-providerapprenticeshipsservice-web";
            _logger = new Mock<ILogger<GetClientContentRequestHandler>>();
            _contentApiClientMock = new Mock<IContentApiClient>();
            _contentApiClientMock
                .Setup(mock => mock.Get(_contentType, _clientId))
                .ReturnsAsync(ContentBanner);

            _request = new GetClientContentRequest
            {
                ContentType = "banner"
            };

            _handler = new GetClientContentRequestHandler( _logger.Object, _contentApiClientMock.Object, ProviderApprenticeshipsServiceConfiguration);
        }


        [Test]
        public async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await _handler.Handle(_request, new CancellationToken());

            //Assert
            _contentApiClientMock.Verify(x => x.Get(_contentType, _clientId), Times.Once);
        }


        [Test]
        public async Task ThenIfShouldUseLegacyStyles_ShouldFetchCorrectContent()
        {
            //Arrange
            _request.UseLegacyStyles = true; 

            //Act
            await _handler.Handle(_request, new CancellationToken());

            //Assert
            _contentApiClientMock.Verify(x => x.Get(_contentType, _clientId + "-legacy"), Times.Once);
        }

        [Test]
        public async Task ThenIfTheMessageIsValidTheValueIsReturnedThenResponseIsSuccess()
        {
            //Act
            var response = await _handler.Handle(_request, new CancellationToken());

            //Assert
            Assert.False(response.HasFailed);
        }

        [Test]
        public async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await _handler.Handle(_request, new CancellationToken());

            //Assert
            Assert.AreEqual(ContentBanner, response.Content);
        }

        [Test]
        public async Task ThenContentApiThrows_ShouldLogError()
        {
            //Arrange
            var cacheKey = $"{ProviderApprenticeshipsServiceConfiguration.ContentApplicationId}_{_request.ContentType}".ToLowerInvariant();
            _contentApiClientMock
                .Setup(mock => mock.Get(_contentType, _clientId))
                .Throws(new Exception("Error"));

            //Act
            var response = await _handler.Handle(_request, new CancellationToken());

            //Assert
            _logger.VerifyLogging($"Failed to get Content for {cacheKey}", LogLevel.Error, Times.Once());
        }

        [Test]
        public async Task ThenContentApiThrows_ShouldReturnHasFailed()
        {
            //Arrange
            _contentApiClientMock
                .Setup(mock => mock.Get(_contentType, _clientId))
                .Throws(new Exception("Error"));


            //Act
            var response = await _handler.Handle(_request, new CancellationToken());

            //Assert
            Assert.IsTrue(response.HasFailed);
        }
    }
}
