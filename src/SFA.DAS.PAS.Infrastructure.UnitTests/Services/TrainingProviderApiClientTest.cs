using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.TrainingProvider;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.PAS.Infrastructure.UnitTests.Services
{
    public class TrainingProviderApiClientTest
    {
        private const string OuterApiBaseAddress = "http://outer-api";
        private MockHttpMessageHandler _mockHttp = null!;
        private Fixture _fixture = null!;
        private TrainingProviderApiClient _sut = null!;
        private TrainingProviderApiClientConfiguration _config = null!;
        private Mock<ILogger<TrainingProviderApiClient>> _logger = null!;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _mockHttp = new MockHttpMessageHandler();
            _config = _fixture
                .Build<TrainingProviderApiClientConfiguration>()
                .With(x =>x.ApiBaseUrl, OuterApiBaseAddress)
                .With(x => x.IdentifierUri, "")
                .Create<TrainingProviderApiClientConfiguration>();
            _logger = new Mock<ILogger<TrainingProviderApiClient>>();

            var client = new HttpClient(_mockHttp)
            {
                BaseAddress = new Uri(OuterApiBaseAddress),
            };
            _sut = new TrainingProviderApiClient(client, _config, _logger.Object);
        }

        [Test]
        public async Task When_ProviderStatus_Found_Then_Data_FromOuterApiIsReturned()
        {
            // Arrange
            var ukprn = _fixture.Create<long>();
            var expected = _fixture.Create<GetProviderStatusResult>();

            _mockHttp.When($"{OuterApiBaseAddress}/api/providers/{ukprn}/validate")
                .Respond("application/json", 
                    JsonSerializer.Serialize(expected));

            // Act
            var actual = await _sut.GetProviderStatus(ukprn);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task When_ProviderStatus_NotFound_Then_Data_FromOuterApiIsNotFound()
        {
            // Arrange
            var ukprn = _fixture.Create<long>();
            _mockHttp.When($"{OuterApiBaseAddress}/api/providers/{ukprn}/validate")
                .Respond("application/json", 
                    JsonSerializer.Serialize((GetProviderStatusResult)null));

            // Act
            var actual = await _sut.GetProviderStatus(ukprn);

            // Assert
            actual.Should().BeNull();
        }

        [Test]
        public async Task When_ProviderStatus_InternalServerError_Then_Data_FromOuterApiIsNotFound()
        {
            // Arrange
            var ukprn = _fixture.Create<long>();

            var error = @"{""Code"":""ServiceUnavailable"", ""Message"" : ""Service Unavailable.""}";

            _mockHttp.When(HttpMethod.Get, $"{OuterApiBaseAddress}/api/providers/{ukprn}/validate")
                .Respond(HttpStatusCode.InternalServerError, "application/json", JsonSerializer.Serialize(error));

            // Act
            var actual = await _sut.GetProviderStatus(ukprn);

            // Assert
            actual.Should().BeNull();
        }
    }
}
