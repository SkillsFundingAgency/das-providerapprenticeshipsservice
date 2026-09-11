using System.Threading.Tasks;
using AutoFixture.NUnit4;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Jobs.Functions;
using SFA.DAS.PAS.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Jobs.UnitTests.Functions;

public class SynchroniseUsersFunctionTests
{
    [Test, MoqAutoData]
    public async Task WhenUpdateUsersFromDfeSignIn_AndSyncSucceeds_ThenCallsUserSyncServiceOnce(
        [Frozen] Mock<IUserSyncService> userSyncService,
        [Greedy] SynchroniseUsersFunction sut)
    {
        await sut.Run(null);

        userSyncService.Verify(x => x.SyncUsers(), Times.Once);
    }
}
