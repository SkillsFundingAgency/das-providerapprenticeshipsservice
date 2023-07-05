using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.PAS.ImportProvider.WebJob.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

namespace SFA.DAS.PAS.ImportProvider.WebJob.UnitTests;

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
        public Mock<ICommitmentsV2ApiClient> CommitmentsV2ApiClient { get; set; }
        public Mock<IProviderRepository> ImportProviderRepository { get; set; }
            
        public WhenImportingProvidersFixture()
        {
            var autoFixture = new Fixture();
            var response = new GetAllProvidersResponse
            {
                Providers = autoFixture.CreateMany<Provider>(1600).ToList()
            };

            CommitmentsV2ApiClient = new Mock<ICommitmentsV2ApiClient>();
            CommitmentsV2ApiClient.Setup(x => x.GetProviders()).ReturnsAsync(response);

            ImportProviderRepository = new Mock<IProviderRepository>();
            ImportProviderRepository.Setup(x => x.ImportProviders(It.IsAny<Provider[]>()));

            Sut = new ImportProviderService(CommitmentsV2ApiClient.Object, ImportProviderRepository.Object, Mock.Of<ILogger<ImportProviderService>>());
        }

        public async Task<WhenImportingProvidersFixture> Import()
        {
            await Sut.Import();
            return this;
        }

        public WhenImportingProvidersFixture VerifyImportProviderRepositoryCalled()
        {
            ImportProviderRepository.Verify(x => x.ImportProviders(It.IsAny<Provider[]>()), Times.Exactly(2));
            return this;
        }
    }
}