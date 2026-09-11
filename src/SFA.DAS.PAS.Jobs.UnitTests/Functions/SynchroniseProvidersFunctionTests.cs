using System.Threading.Tasks;
using AutoFixture.NUnit4;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Jobs.Functions;
using SFA.DAS.PAS.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Jobs.UnitTests.Functions;

public class SynchroniseProvidersFunctionTests
{
    [Test, MoqAutoData]
    public async Task WhenSynchronisingProviders_InvokesImportProviderService(
        [Frozen] Mock<IImportProviderService> importProviderService,
        [Greedy] SynchroniseProvidersFunction sut)
    {
        await sut.Run(null);

        importProviderService.Verify(x => x.ImportProviders(), Times.Once);
    }
}
