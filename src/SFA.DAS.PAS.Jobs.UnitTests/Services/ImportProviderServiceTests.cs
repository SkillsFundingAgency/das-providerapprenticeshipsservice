using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Jobs.ApiClients;
using SFA.DAS.PAS.Jobs.ApiModels;
using SFA.DAS.PAS.Jobs.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Jobs.UnitTests.Services;

public class ImportProviderServiceTests
{
    [Test, MoqAutoData]
    public async Task WhenImport_AndProvidersReturnedInBatches_ThenCallsRepositoryForEachBatch(
        [Frozen] Mock<IRoatpApiClient> roatpApiClient,
        [Frozen] Mock<IProviderRepository> providerRepository,
        [Greedy] ImportProviderService sut)
    {
        var providers = Enumerable.Range(1, 1600)
            .Select(i => new Provider { Ukprn = i, LegalName = $"Provider {i}" })
            .ToList();

        roatpApiClient
            .Setup(x => x.GetProviders())
            .ReturnsAsync(new GetAllProvidersResponse { Organisations = providers });

        await sut.ImportProviders();

        providerRepository.Verify(x => x.ImportProviders(It.IsAny<CommitmentsV2.Api.Types.Responses.Provider[]>()), Times.Exactly(2));
    }

    [Test, MoqAutoData]
    public async Task WhenImport_AndNoProvidersReturned_ThenDoesNotCallRepository(
        [Frozen] Mock<IRoatpApiClient> roatpApiClient,
        [Frozen] Mock<IProviderRepository> providerRepository,
        [Greedy] ImportProviderService sut)
    {
        roatpApiClient
            .Setup(x => x.GetProviders())
            .ReturnsAsync(new GetAllProvidersResponse { Organisations = [] });

        await sut.ImportProviders();

        providerRepository.Verify(x => x.ImportProviders(It.IsAny<CommitmentsV2.Api.Types.Responses.Provider[]>()), Times.Never);
    }
}
