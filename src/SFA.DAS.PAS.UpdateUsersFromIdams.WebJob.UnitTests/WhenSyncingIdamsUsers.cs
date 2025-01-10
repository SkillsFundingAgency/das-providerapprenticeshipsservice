using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.DfESignInUser;
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

            var users = autoFixture.Build<User>().With(c => c.UserStatus , 1).CreateMany().ToList();
            
            _normalUsers = autoFixture.Build<DfeUser>().With(c=>c.Users, users).Create();
            
            _providerRepository = new Mock<IProviderRepository>();
            _providerRepository.Setup(x => x.GetNextProviderForIdamsUpdate()).ReturnsAsync(_providerResponse);
            _userRepository = new Mock<IUserRepository>();

            _apiHelper = new Mock<IApiHelper>();
            _apiHelper
                .Setup(x => x.Get<DfeUser>(It.IsAny<string>())).ReturnsAsync(_normalUsers);

            _configuration = new DfEOidcConfiguration
            {
                APIServiceUrl = "https://some.test.url"
            };

            Sut = new IdamsSyncService(_userRepository.Object,
                _providerRepository.Object, Mock.Of<ILogger<IdamsSyncService>>(), _apiHelper.Object, _configuration);
        }

        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IProviderRepository> _providerRepository;
        private readonly Provider _providerResponse;
        private readonly DfeUser _normalUsers;
        private readonly Mock<IApiHelper> _apiHelper;
        private readonly DfEOidcConfiguration _configuration;

        public IdamsSyncService Sut { get; }

        public WhenSyncingIdamsUsersFixture SetupIdamsToThrowException()
        {
            _apiHelper
                .Setup(x => x.Get<DfeUser>(It.IsAny<string>()))
                .Throws<ApplicationException>();
            return this;
        }

        public WhenSyncingIdamsUsersFixture SetupIdamsToThrowHttpRequestException()
        {
            _apiHelper
                .Setup(x => x.Get<DfeUser>(It.IsAny<string>()))
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
            _apiHelper.Verify(x => x.Get<DfeUser>($"{_configuration.APIServiceUrl}/organisations/{_providerResponse.Ukprn}/users"), Times.Once);
        }

        public void VerifyIdamsUsersAreSyncedInUserRepository()
        {
            
            _userRepository.Verify(x => x.SyncIdamsUsers(It.IsAny<long>(),
                It.Is<List<IdamsUser>>(p => p.Count(z => z.UserType == UserType.NormalUser) == _normalUsers.Users.Count)));
        }

        public void VerifyItMarksProviderAsIdamsUpdated()
        {
            _providerRepository.Verify(x => x.MarkProviderIdamsUpdated(_providerResponse.Ukprn));
        }

        public void VerifyIdamsServiceIsNotCalled()
        {
            _apiHelper.Verify(x => x.Get<DfeUser>(It.IsAny<string>()), Times.Never);
        }
    }
}