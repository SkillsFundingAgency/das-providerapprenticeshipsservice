using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using Moq;
using NUnit.Framework;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.PAS.Jobs.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.DfESignInUser;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.IdamsUser;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PAS.Jobs.UnitTests.Services;

public class UserSyncServiceTests
{
    [Test, MoqAutoData]
    public async Task WhenSyncUsers_AndProviderExists_ThenGetsNextProviderToProcess(
        [Frozen] Mock<IProviderRepository> providerRepository,
        [Frozen] Mock<IApiHelper> apiHelper,
        [Frozen] DfEOidcConfiguration configuration,
        [Greedy] UserSyncService sut,
        Provider provider,
        DfeUser dfeUser)
    {
        ArrangeHappyPath(providerRepository, apiHelper, provider, dfeUser);

        await sut.SyncUsers();

        providerRepository.Verify(x => x.GetNextProviderForIdamsUpdate(), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenSyncUsers_AndProviderExists_ThenCallsUserSyncService(
        [Frozen] Mock<IProviderRepository> providerRepository,
        [Frozen] Mock<IApiHelper> apiHelper,
        [Frozen] DfEOidcConfiguration configuration,
        [Greedy] UserSyncService sut,
        Provider provider,
        DfeUser dfeUser)
    {
        ArrangeHappyPath(providerRepository, apiHelper, provider, dfeUser);

        await sut.SyncUsers();

        apiHelper.Verify(
            x => x.Get<DfeUser>($"{configuration.APIServiceUrl}/organisations/{provider.Ukprn}/users"),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenSyncUsers_AndUsersReturned_ThenSyncsUsersWithLocalRepository(
        [Frozen] Mock<IProviderRepository> providerRepository,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IApiHelper> apiHelper,
        [Frozen] DfEOidcConfiguration configuration,
        [Greedy] UserSyncService sut,
        Provider provider,
        DfeUser dfeUser)
    {
        ArrangeHappyPath(providerRepository, apiHelper, provider, dfeUser);

        await sut.SyncUsers();

        userRepository.Verify(
            x => x.SyncIdamsUsers(
                It.IsAny<long>(),
                It.Is<List<IdamsUser>>(p => p.Count(z => z.UserType == UserType.NormalUser) == dfeUser.Users.Count)),
            Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenSyncUsers_AndProviderProcessed_ThenMarksProviderAsUpdated(
        [Frozen] Mock<IProviderRepository> providerRepository,
        [Frozen] Mock<IApiHelper> apiHelper,
        [Frozen] DfEOidcConfiguration configuration,
        [Greedy] UserSyncService sut,
        Provider provider,
        DfeUser dfeUser)
    {
        ArrangeHappyPath(providerRepository, apiHelper, provider, dfeUser);

        await sut.SyncUsers();

        providerRepository.Verify(x => x.MarkProviderIdamsUpdated(provider.Ukprn), Times.Once);
    }

    [Test, MoqAutoData]
    public void WhenSyncUsers_AndIdamsThrowsException_ThenRethrowsException(
        [Frozen] Mock<IProviderRepository> providerRepository,
        [Frozen] Mock<IApiHelper> apiHelper,
        [Frozen] DfEOidcConfiguration configuration,
        [Greedy] UserSyncService sut,
        Provider provider)
    {
        providerRepository.Setup(x => x.GetNextProviderForIdamsUpdate()).ReturnsAsync(provider);
        apiHelper.Setup(x => x.Get<DfeUser>(It.IsAny<string>())).Throws<ApplicationException>();

        Assert.ThrowsAsync<ApplicationException>(() => sut.SyncUsers());
    }

    [Test, MoqAutoData]
    public void WhenSyncUsers_AndIdamsThrowsException_ThenMarksProviderAsUpdated(
        [Frozen] Mock<IProviderRepository> providerRepository,
        [Frozen] Mock<IApiHelper> apiHelper,
        [Frozen] DfEOidcConfiguration configuration,
        [Greedy] UserSyncService sut,
        Provider provider)
    {
        providerRepository.Setup(x => x.GetNextProviderForIdamsUpdate()).ReturnsAsync(provider);
        apiHelper.Setup(x => x.Get<DfeUser>(It.IsAny<string>())).Throws<ApplicationException>();

        Assert.ThrowsAsync<ApplicationException>(() => sut.SyncUsers());

        providerRepository.Verify(x => x.MarkProviderIdamsUpdated(provider.Ukprn), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenSyncUsers_AndNoProvidersFound_ThenDoesNotCallIdamsService(
        [Frozen] Mock<IProviderRepository> providerRepository,
        [Frozen] Mock<IApiHelper> apiHelper,
        [Frozen] DfEOidcConfiguration configuration,
        [Greedy] UserSyncService sut)
    {
        providerRepository.Setup(x => x.GetNextProviderForIdamsUpdate()).ReturnsAsync((Provider)null);

        await sut.SyncUsers();

        apiHelper.Verify(x => x.Get<DfeUser>(It.IsAny<string>()), Times.Never);
    }

    [Test, MoqAutoData]
    public void WhenSyncUsers_AndIdamsThrowsHttp404Exception_ThenMarksProviderAsUpdatedButDoesNotThrow(
        [Frozen] Mock<IProviderRepository> providerRepository,
        [Frozen] Mock<IApiHelper> apiHelper,
        [Frozen] DfEOidcConfiguration configuration,
        [Greedy] UserSyncService sut,
        Provider provider)
    {
        providerRepository.Setup(x => x.GetNextProviderForIdamsUpdate()).ReturnsAsync(provider);
        apiHelper
            .Setup(x => x.Get<DfeUser>(It.IsAny<string>()))
            .Throws(new CustomHttpRequestException(HttpStatusCode.NotFound, null));

        Assert.DoesNotThrowAsync(() => sut.SyncUsers());

        providerRepository.Verify(x => x.MarkProviderIdamsUpdated(provider.Ukprn), Times.Once);
    }

    [Test, MoqAutoData]
    public void WhenSyncUsers_AndIdamsThrowsNon404HttpException_ThenRethrowsAndMarksProviderAsUpdated(
        [Frozen] Mock<IProviderRepository> providerRepository,
        [Frozen] Mock<IApiHelper> apiHelper,
        [Frozen] DfEOidcConfiguration configuration,
        [Greedy] UserSyncService sut,
        Provider provider)
    {
        providerRepository.Setup(x => x.GetNextProviderForIdamsUpdate()).ReturnsAsync(provider);
        apiHelper
            .Setup(x => x.Get<DfeUser>(It.IsAny<string>()))
            .Throws(new CustomHttpRequestException(HttpStatusCode.InternalServerError, null));

        Assert.ThrowsAsync<CustomHttpRequestException>(() => sut.SyncUsers());

        providerRepository.Verify(x => x.MarkProviderIdamsUpdated(provider.Ukprn), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task WhenSyncUsers_AndIdamsReturnsNull_ThenSyncsEmptyUserList(
        [Frozen] Mock<IProviderRepository> providerRepository,
        [Frozen] Mock<IUserRepository> userRepository,
        [Frozen] Mock<IApiHelper> apiHelper,
        [Frozen] DfEOidcConfiguration configuration,
        [Greedy] UserSyncService sut,
        Provider provider)
    {
        providerRepository.Setup(x => x.GetNextProviderForIdamsUpdate()).ReturnsAsync(provider);
        apiHelper.Setup(x => x.Get<DfeUser>(It.IsAny<string>())).ReturnsAsync((DfeUser)null);

        await sut.SyncUsers();

        userRepository.Verify(
            x => x.SyncIdamsUsers(provider.Ukprn, It.Is<List<IdamsUser>>(p => p.Count == 0)),
            Times.Once);
        providerRepository.Verify(x => x.MarkProviderIdamsUpdated(provider.Ukprn), Times.Once);
    }

    private static void ArrangeHappyPath(
        Mock<IProviderRepository> providerRepository,
        Mock<IApiHelper> apiHelper,
        Provider provider,
        DfeUser dfeUser)
    {
        foreach (var user in dfeUser.Users)
        {
            user.UserStatus = 1;
        }

        providerRepository.Setup(x => x.GetNextProviderForIdamsUpdate()).ReturnsAsync(provider);
        apiHelper.Setup(x => x.Get<DfeUser>(It.IsAny<string>())).ReturnsAsync(dfeUser);
    }
}
