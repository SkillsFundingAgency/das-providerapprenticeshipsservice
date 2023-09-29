using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.TrainingProvider;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.PAS.Infrastructure.UnitTests.Services
{
    public class TrainingProviderApiClientTest
    {
        private const string OuterApiBaseAddress = "http://outer-api";
        private Mock<HttpMessageHandler> _mockHttpsMessageHandler = null!;
        private Fixture _fixture = null!;
        private TrainingProviderApiClient _sut = null!;
        private TrainingProviderApiClientConfiguration _config = null!;
        private Mock<ILogger<TrainingProviderApiClient>> _logger = null!;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _config = _fixture
                .Build<TrainingProviderApiClientConfiguration>()
                .With(x =>x.ApiBaseUrl, OuterApiBaseAddress)
                .With(x => x.IdentifierUri, "")
                .Create();
            _logger = new Mock<ILogger<TrainingProviderApiClient>>();
            _mockHttpsMessageHandler = new Mock<HttpMessageHandler>();
        }

        [Test]
        public async Task When_ProviderDetails_Found_Then_Data_FromOuterApiIsReturned()
        {
            // Arrange
            var ukprn = _fixture.Create<long>();
            var expected = _fixture.Create<GetProviderSummaryResult>();

            _mockHttpsMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(expected)),
                    RequestMessage = new HttpRequestMessage()
                });
            var httpClient = new HttpClient(_mockHttpsMessageHandler.Object)
            {
                BaseAddress = new Uri(OuterApiBaseAddress),
            };
            _sut = new TrainingProviderApiClient(httpClient, _config, _logger.Object);

            // Act
            var actual = await _sut.GetProviderDetails(ukprn);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task When_ProviderDetails_NotFound_Then_Data_FromOuterApiIsNotFound()
        {
            // Arrange
            var ukprn = _fixture.Create<long>();
            _mockHttpsMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(""),
                    RequestMessage = new HttpRequestMessage()
                });
            var httpClient = new HttpClient(_mockHttpsMessageHandler.Object)
            {
                BaseAddress = new Uri(OuterApiBaseAddress),
            };
            _sut = new TrainingProviderApiClient(httpClient, _config, _logger.Object);

            // Act
            var actual = await _sut.GetProviderDetails(ukprn);

            // Assert
            actual.Should().BeNull();
        }

        [Test]
        public async Task When_ProviderDetails_InternalServerError_Then_Data_FromOuterApiIsNotFound()
        {
            // Arrange
            var ukprn = _fixture.Create<long>();

            _mockHttpsMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(""),
                    RequestMessage = new HttpRequestMessage()
                });
            var httpClient = new HttpClient(_mockHttpsMessageHandler.Object)
            {
                BaseAddress = new Uri(OuterApiBaseAddress),
            };
            _sut = new TrainingProviderApiClient(httpClient, _config, _logger.Object);

            // Act
            var actual = await _sut.GetProviderDetails(ukprn);

            // Assert
            actual.Should().BeNull();
        }
    }
}
