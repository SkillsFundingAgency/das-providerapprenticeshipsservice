using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types.Providers;
using SFA.DAS.NLog.Logger;
using SFA.DAS.PAS.ImportProvider.WebJob.Importer;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.Providers.Api.Client;

namespace SFA.DAS.PAS.ImportProviders.WebJob.UnitTests
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
            public ImportProviderService Sut { get; set; }
            public Mock<IProviderApiClient> providerApiClient { get; set; }
            public Mock<IImportProviderRepository> importProviderRepository { get; set; }
            public IEnumerable<ProviderSummary> providerApiClientResponse { get; set; }

            public WhenImportingProvidersFixture()
            {
                var autoFixture = new Fixture();
                providerApiClientResponse = autoFixture.CreateMany<ProviderSummary>(1600);

                providerApiClient = new Mock<IProviderApiClient>();
                providerApiClient.Setup(x => x.FindAllAsync()).ReturnsAsync(providerApiClientResponse);

                importProviderRepository = new Mock<IImportProviderRepository>();
                importProviderRepository.Setup(x => x.ImportProviders(It.IsAny<DataTable>()));

                Sut = new ImportProviderService(providerApiClient.Object, importProviderRepository.Object, Mock.Of<ILog>());
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
