using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Http;
using SFA.DAS.PAS.Account.Api.ClientV2;
using SFA.DAS.PAS.Account.Api.TypesV2;

namespace SFA.DAS.PAS.Account.Api.Client.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    public class WhenCallingPasAccountApiClient
    {
        [Test]
        public async Task GetUserRef_VerifyUrlAndDataIsCorrectPassedIn()
        {
            var fixture = new WhenCallingPasAccountApiClientFixture();
            await fixture.PasAccountApiClient.GetUser(fixture.UserRef, CancellationToken.None);
            fixture.MockRestHttpClient.Verify(x => x.Get<User>($"api/user/{fixture.UserRef}", null, CancellationToken.None));
        }

        [Test]
        public async Task GetUserRef_VerifyUserIsReturned()
        {
            var fixture = new WhenCallingPasAccountApiClientFixture();
            var result = await fixture.SetupResponseForGetUser().PasAccountApiClient.GetUser(fixture.UserRef, CancellationToken.None);
            Assert.AreEqual(fixture.User, result);
        }

        [Test]
        public async Task GetAccountUsers_VerifyUrlAndDataIsCorrectPassedIn()
        {
            var fixture = new WhenCallingPasAccountApiClientFixture();
            await fixture.PasAccountApiClient.GetAccountUsers(fixture.ProviderId, CancellationToken.None);
            fixture.MockRestHttpClient.Verify(x => x.Get<IEnumerable<User>>($"api/account/{fixture.ProviderId}/users", null, CancellationToken.None));
        }

        [Test]
        public async Task GetAccountUsers_VerifyUserListIsReturned()
        {
            var fixture = new WhenCallingPasAccountApiClientFixture();
            var result = await fixture.SetupResponseForGetAccountUsers().PasAccountApiClient.GetAccountUsers(fixture.ProviderId, CancellationToken.None);
            Assert.AreEqual(fixture.Users, result);
        }

        [Test]        
        public async Task SendEmailToAllProviderRecipients_VerifyUrlAndDataIsCorrectPassedIn()
        {
            var fixture = new WhenCallingPasAccountApiClientFixture();
            await fixture.PasAccountApiClient.SendEmailToAllProviderRecipients(fixture.ProviderId, fixture.ProviderEmailRequest, CancellationToken.None);
            fixture.MockRestHttpClient.Verify(x => x.PostAsJson<ProviderEmailRequest>($"api/email/{fixture.ProviderId}/send", fixture.ProviderEmailRequest, CancellationToken.None));
        }
    }

    public class WhenCallingPasAccountApiClientFixture
    {
        public PasAccountApiClient PasAccountApiClient { get; }
        public Mock<IRestHttpClient> MockRestHttpClient { get; }
        public long ProviderId { get; }
        public string UserRef { get; }
        public User User{ get; }
        public List<User> Users { get; }
        public ProviderEmailRequest ProviderEmailRequest { get; }

        public WhenCallingPasAccountApiClientFixture()
        {
            var autoFixture = new Fixture();
            MockRestHttpClient = new Mock<IRestHttpClient>();
            UserRef = autoFixture.Create<string>();
            User = autoFixture.Create<User>();
            Users = new List<User>();
            PasAccountApiClient = new PasAccountApiClient(MockRestHttpClient.Object);
            ProviderEmailRequest = autoFixture.Create<ProviderEmailRequest>();
        }

        public WhenCallingPasAccountApiClientFixture SetupResponseForGetUser()
        {
            MockRestHttpClient.Setup(x => x.Get<User>(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(User);
            return this;
        }

        public WhenCallingPasAccountApiClientFixture SetupResponseForGetAccountUsers()
        {
            MockRestHttpClient.Setup(x => x.Get<IEnumerable<User>>(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Users);
            return this;
        }
    }
}