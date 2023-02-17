using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System.Data;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.PAS.ImportProvider.WebJob.Services;
using Microsoft.Extensions.Logging;

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
            public Mock<IProviderCommitmentsApi> providerApiClient { get; set; }
            public Mock<IProviderRepository> importProviderRepository { get; set; }
            
            public WhenImportingProvidersFixture()
            {
                var autoFixture = new Fixture();
                var response = new GetProvidersResponse();
                response.Providers = autoFixture.CreateMany<ProviderResponse>(1600);
                
                providerApiClient = new Mock<IProviderCommitmentsApi>();
                providerApiClient.Setup(x => x.GetProviders()).ReturnsAsync(response);

                importProviderRepository = new Mock<IProviderRepository>();
                importProviderRepository.Setup(x => x.ImportProviders(It.IsAny<DataTable>()));

                Sut = new ImportProviderService(providerApiClient.Object, importProviderRepository.Object, Mock.Of<ILogger<ImportProviderService>>());
            }

            public async Task<WhenImportingProvidersFixture> Import()
            {
                await Sut.Import();
                return this;
            }

            public WhenImportingProvidersFixture VerifyImportProviderRepositoryCalled()
            {
                importProviderRepository.Verify(x => x.ImportProviders(It.IsAny<DataTable>()), Times.Exactly(2));
                return this;
            }
        }
    }
}
