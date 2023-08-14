using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.IdamsUser;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.UnitTests;

[TestFixture]
public class WhenSyncingIdamsUsers
{
    [Test]
    public async Task Then_WeGetNextProviderToProcess()
    {
        var f = new WhenSyncingIdamsUsersFixture();
        await f.Sut.SyncUsers();
        f.VerifyItGetsTheNextProvider();
    }

    [Test]
    public async Task Then_WeCallIdamsServiceForThisProvider()
    {
        var f = new WhenSyncingIdamsUsersFixture();
        await f.Sut.SyncUsers();
        f.VerifyWeCallIdamsServiceForThisProvider();
    }

    [Test]
    public async Task Then_TheNormalAndSuperUsersAreSyncedWithLocalUsers()
    {
        var f = new WhenSyncingIdamsUsersFixture();
        await f.Sut.SyncUsers();
        f.VerifyIdamsUsersAreSyncedInUserRepository();
    }

    [Test]
    public async Task Then_WeMarkProviderAsUpdated()
    {
        var f = new WhenSyncingIdamsUsersFixture();
        await f.Sut.SyncUsers();
        f.VerifyItMarksProviderAsIdamsUpdated();
    }

    [Test]
    public void AndWhenIdamsThrowsAnException_Then_TheExceptionIsRethrown()
    {
        var f = new WhenSyncingIdamsUsersFixture().SetupIdamsToThrowException();
        Assert.ThrowsAsync<ApplicationException>(() => f.Sut.SyncUsers());
    }

    [Test]
    public void AndWhenIdamsThrowsAnException_Then_WeStillMarkProviderAsUpdated()
    {
        var f = new WhenSyncingIdamsUsersFixture().SetupIdamsToThrowException();
        Assert.ThrowsAsync<ApplicationException>(() => f.Sut.SyncUsers());
        f.VerifyItMarksProviderAsIdamsUpdated();
    }

    [Test]
    public async Task AndWhenThereAreNoProviders_Then_WeDontCallTheIdamsService()
    {
        var f = new WhenSyncingIdamsUsersFixture().WithNoProviders();
        await f.Sut.SyncUsers();
        f.VerifyIdamsServiceIsNotCalled();
    }

    [Test]
    public void AndWhenIdamsThrowsAnHttp404RequestException_Then_WeStillMarkProviderAsUpdatedButWeDoNotThrowException()
    {
        var f = new WhenSyncingIdamsUsersFixture().SetupIdamsToThrowHttpRequestException();
        Assert.DoesNotThrowAsync(() => f.Sut.SyncUsers());
        f.VerifyItMarksProviderAsIdamsUpdated();
    }

    public class WhenSyncingIdamsUsersFixture
    {
        public WhenSyncingIdamsUsersFixture()
        {
            var autoFixture = new Fixture();
            _providerResponse = autoFixture.Create<Provider>();
            _superUsers = autoFixture.CreateMany<string>().ToList();
            _normalUsers = autoFixture.CreateMany<string>().ToList();
            var combinedUsers = _normalUsers.Concat(_superUsers).ToList();

            _providerRepository = new Mock<IProviderRepository>();
            _providerRepository.Setup(x => x.GetNextProviderForIdamsUpdate()).ReturnsAsync(_providerResponse);

            _idamsEmailServiceWrapper = new Mock<IIdamsEmailServiceWrapper>();
            _idamsEmailServiceWrapper.Setup(x => x.GetEmailsAsync(It.IsAny<long>(), "UserRole"))
                .ReturnsAsync(combinedUsers);
            
            _idamsEmailServiceWrapper.Setup(x => x.GetEmailsAsync(It.IsAny<long>(), "SuperUserRole"))
                .ReturnsAsync(_superUsers);

            _userRepository = new Mock<IUserRepository>();

            var configuration = new ProviderNotificationConfiguration
            {
                DasUserRoleId = "UserRole",
                SuperUserRoleId = "SuperUserRole"
            };

            Sut = new IdamsSyncService(_idamsEmailServiceWrapper.Object, _userRepository.Object,
                _providerRepository.Object, Mock.Of<ILogger<IdamsSyncService>>(), configuration);
        }

        private readonly Mock<IIdamsEmailServiceWrapper> _idamsEmailServiceWrapper;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IProviderRepository> _providerRepository;
        private readonly Provider _providerResponse;
        private readonly List<string> _superUsers;
        private readonly List<string> _normalUsers;

        public IdamsSyncService Sut { get; }

        public WhenSyncingIdamsUsersFixture SetupIdamsToThrowException()
        {
            _idamsEmailServiceWrapper.Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .Throws<ApplicationException>();
            return this;
        }

        public WhenSyncingIdamsUsersFixture SetupIdamsToThrowHttpRequestException()
        {
            _idamsEmailServiceWrapper
                .Setup(x => x.GetEmailsAsync(It.IsAny<long>(), It.IsAny<string>()))
                .Throws(new CustomHttpRequestException(HttpStatusCode.NotFound, null));

            return this;
        }

        public WhenSyncingIdamsUsersFixture WithNoProviders()
        {
            _providerRepository.Setup(x => x.GetNextProviderForIdamsUpdate()).ReturnsAsync((Provider)null);
            return this;
        }


        public void VerifyItGetsTheNextProvider()
        {
            _providerRepository.Verify(x => x.GetNextProviderForIdamsUpdate());
        }

        public void VerifyWeCallIdamsServiceForThisProvider()
        {
            _idamsEmailServiceWrapper.Verify(x => x.GetEmailsAsync(_providerResponse.Ukprn, "UserRole"));
            _idamsEmailServiceWrapper.Verify(x => x.GetEmailsAsync(_providerResponse.Ukprn, "SuperUserRole"));
        }

        public void VerifyIdamsUsersAreSyncedInUserRepository()
        {
            _userRepository.Verify(x => x.SyncIdamsUsers(_providerResponse.Ukprn,
                It.Is<List<IdamsUser>>(p => p.Count == _normalUsers.Count + _superUsers.Count)));
            
            _userRepository.Verify(x => x.SyncIdamsUsers(It.IsAny<long>(),
                It.Is<List<IdamsUser>>(p => p.Count(z => z.UserType == UserType.SuperUser) == _superUsers.Count)));
            
            _userRepository.Verify(x => x.SyncIdamsUsers(It.IsAny<long>(),
                It.Is<List<IdamsUser>>(p => p.Count(z => z.UserType == UserType.NormalUser) == _normalUsers.Count)));
        }

        public void VerifyItMarksProviderAsIdamsUpdated()
        {
            _providerRepository.Verify(x => x.MarkProviderIdamsUpdated(_providerResponse.Ukprn));
        }

        public void VerifyIdamsServiceIsNotCalled()
        {
            _idamsEmailServiceWrapper.Verify(x => x.GetEmailsAsync(It.IsAny<long>(), "UserRole"), Times.Never);
            _idamsEmailServiceWrapper.Verify(x => x.GetEmailsAsync(It.IsAny<long>(), "SuperUserRole"), Times.Never);
        }
    }
}