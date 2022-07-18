using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Services
{
    [TestFixture]
    public class ApprenticeshipInfoServiceTests
    {
        public ApprenticeshipInfoService _sut;

        private Mock<ICommitmentsV2ApiClient> _commitmentsV2ApiClient;
        private Mock<ICache> _cache;
        private Mock<ITrainingProgrammeApi> _trainingProgrammeApi;

        [SetUp]
        public void Arrange()
        {
            _commitmentsV2ApiClient = new Mock<ICommitmentsV2ApiClient>();
            _cache = new Mock<ICache>();
            _trainingProgrammeApi = new Mock<ITrainingProgrammeApi>();

            _sut = new ApprenticeshipInfoService(_cache.Object, _commitmentsV2ApiClient.Object, _trainingProgrammeApi.Object);
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
            Assert.AreEqual(result.Provider.ProviderName, response.Name);
            Assert.AreEqual(result.Provider.Ukprn, response.ProviderId);
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
            Assert.IsNull(result);
        }
    }
}