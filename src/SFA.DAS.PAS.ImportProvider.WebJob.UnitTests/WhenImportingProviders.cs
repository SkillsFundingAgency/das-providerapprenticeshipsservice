using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System.Threading.Tasks;
using SFA.DAS.PAS.ImportProvider.WebJob.Services;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using System.Linq;

namespace SFA.DAS.PAS.ImportProvider.WebJob.UnitTests
{
    [TestFixture]
    public class WhenImportingProviders
    {
        [Test]
        public async Task ProvidersAreImportedAndStoredInDb()
        {
            var fixture = new WhenImportingProvidersFixture();
            await fixture.Import();
            fixture.VerifyImportProviderRepositoryCalled();
        }

        public class WhenImportingProvidersFixture
        {
            public IImportProviderService Sut { get; set; }
            public Mock<ICommitmentsV2ApiClient> commitmentsV2ApiClient { get; set; }
            public Mock<IProviderRepository> importProviderRepository { get; set; }
            
            public WhenImportingProvidersFixture()
            {
                var autoFixture = new Fixture();
                var response = new GetAllProvidersResponse();
                response.Providers = autoFixture.CreateMany<Provider>(1600).ToList();

                commitmentsV2ApiClient = new Mock<ICommitmentsV2ApiClient>();
                commitmentsV2ApiClient.Setup(x => x.GetProviders()).ReturnsAsync(response);

                importProviderRepository = new Mock<IProviderRepository>();
                importProviderRepository.Setup(x => x.ImportProviders(It.IsAny<Provider[]>()));

                Sut = new ImportProviderService(commitmentsV2ApiClient.Object, importProviderRepository.Object, Mock.Of<ILogger<ImportProviderService>>());
            }

            public async Task<WhenImportingProvidersFixture> Import()
            {
                await Sut.Import();
                return this;
            }

            public WhenImportingProvidersFixture VerifyImportProviderRepositoryCalled()
            {
                importProviderRepository.Verify(x => x.ImportProviders(It.IsAny<Provider[]>()), Times.Exactly(2));
                return this;
            }
        }
    }
}
