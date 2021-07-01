using System;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.UnitTests
{
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
            Assert.ThrowsAsync<ApplicationException>( () =>  f.Sut.SyncUsers());
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
            public Mock<IIdamsEmailServiceWrapper> IdamsEmailServiceWrapper { get; set; }
            public Mock<IUserRepository> UserRepository { get; set; }
            public Mock<IProviderRepository> ProviderRepository { get; set; }
            public Provider ProviderResponse { get; set; }
            public List<string> SuperUsers { get; set; }
            public List<string> NormalUsers { get; set; }
            public List<string> CombinedUsers { get; set; }

            public IdamsSyncService Sut { get; set; }

            public WhenSyncingIdamsUsersFixture()
            {
                var autoFixture = new Fixture();
                ProviderResponse = autoFixture.Create<Provider>();
                SuperUsers = autoFixture.CreateMany<string>().ToList();
                NormalUsers = autoFixture.CreateMany<string>().ToList();
                CombinedUsers = NormalUsers.Concat(SuperUsers).ToList();

                ProviderRepository = new Mock<IProviderRepository>();
                ProviderRepository.Setup(x => x.GetNextProviderForIdamsUpdate()).ReturnsAsync(ProviderResponse);

                IdamsEmailServiceWrapper = new Mock<IIdamsEmailServiceWrapper>();
                IdamsEmailServiceWrapper.Setup(x => x.GetEmailsAsync(It.IsAny<long>())).ReturnsAsync(CombinedUsers);
                IdamsEmailServiceWrapper.Setup(x => x.GetSuperUserEmailsAsync(It.IsAny<long>())).ReturnsAsync(SuperUsers);

                UserRepository = new Mock<IUserRepository>();

                Sut = new IdamsSyncService(IdamsEmailServiceWrapper.Object, UserRepository.Object, ProviderRepository.Object, Mock.Of<ILog>());
            }

            public WhenSyncingIdamsUsersFixture SetupIdamsToThrowException()
            {
                IdamsEmailServiceWrapper.Setup(x => x.GetEmailsAsync(It.IsAny<long>())).Throws<ApplicationException>();
                return this;
            }
            public WhenSyncingIdamsUsersFixture SetupIdamsToThrowHttpRequestException()
            {
                IdamsEmailServiceWrapper
                    .Setup(x => x.GetEmailsAsync(It.IsAny<long>()))
                    .Throws(new HttpRequestException("Response status code does not indicate success: 404 (Not Found)."));
                return this;
            }

            public WhenSyncingIdamsUsersFixture WithNoProviders()
            {
                ProviderRepository.Setup(x => x.GetNextProviderForIdamsUpdate()).ReturnsAsync((Provider)null);
                return this;
            }


            public void VerifyItGetsTheNextProvider()
            {
                ProviderRepository.Verify(x=>x.GetNextProviderForIdamsUpdate());
            }

            public void VerifyWeCallIdamsServiceForThisProvider()
            {
               IdamsEmailServiceWrapper.Verify(x=>x.GetEmailsAsync(ProviderResponse.Ukprn));
               IdamsEmailServiceWrapper.Verify(x=>x.GetSuperUserEmailsAsync(ProviderResponse.Ukprn));
            }

            public void VerifyIdamsUsersAreSyncedInUserRepository()
            {
                UserRepository.Verify(x => x.SyncIdamsUsers(ProviderResponse.Ukprn, It.Is<List<IdamsUser>>(p => p.Count == NormalUsers.Count() + SuperUsers.Count())));
                UserRepository.Verify(x => x.SyncIdamsUsers(It.IsAny<long>(), It.Is<List<IdamsUser>>(p => p.Count(z => z.UserType == UserType.SuperUser) == SuperUsers.Count())));
                UserRepository.Verify(x => x.SyncIdamsUsers(It.IsAny<long>(), It.Is<List<IdamsUser>>(p => p.Count(z => z.UserType == UserType.NormalUser) == NormalUsers.Count())));
            }

            public void VerifyItMarksProviderAsIdamsUpdated()
            {
                ProviderRepository.Verify(x => x.MarkProviderIdamsUpdated(ProviderResponse.Ukprn));
            }

            public void VerifyIdamsServiceIsNotCalled()
            {
                IdamsEmailServiceWrapper.Verify(x => x.GetEmailsAsync(It.IsAny<long>()), Times.Never);
                IdamsEmailServiceWrapper.Verify(x => x.GetSuperUserEmailsAsync(It.IsAny<long>()), Times.Never);
            }
        }
    }
}