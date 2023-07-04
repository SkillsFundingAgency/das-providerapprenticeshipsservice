using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Http;
using SFA.DAS.PAS.Account.Api.Types;

namespace SFA.DAS.PAS.Account.Api.ClientV2.UnitTests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
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
        var fixture = new WhenCallingPasAccountApiClientFixture().SetupResponseForGetUser();
        var result = await fixture.PasAccountApiClient.GetUser(fixture.UserRef, CancellationToken.None);
        Assert.That(fixture.User, Is.EqualTo(result));
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
        var fixture = new WhenCallingPasAccountApiClientFixture().SetupResponseForGetAccountUsers();
        var result = await fixture.PasAccountApiClient.GetAccountUsers(fixture.ProviderId, CancellationToken.None);
        Assert.That(fixture.Users, Is.EqualTo(result));
    }

    [Test]        
    public async Task SendEmailToAllProviderRecipients_VerifyUrlAndDataIsCorrectPassedIn()
    {
        var fixture = new WhenCallingPasAccountApiClientFixture();
        await fixture.PasAccountApiClient.SendEmailToAllProviderRecipients(fixture.ProviderId, fixture.ProviderEmailRequest, CancellationToken.None);
        fixture.MockRestHttpClient.Verify(x => x.PostAsJson<ProviderEmailRequest>($"api/email/{fixture.ProviderId}/send", fixture.ProviderEmailRequest, CancellationToken.None));
    }

    [Test]
    public async Task GetAgreement_VerifyUrlAndDataIsCorrectPassedIn()
    {
        var fixture = new WhenCallingPasAccountApiClientFixture();
        await fixture.PasAccountApiClient.GetAgreement(fixture.ProviderId, CancellationToken.None);
        fixture.MockRestHttpClient.Verify(x => x.Get<ProviderAgreement>($"api/account/{fixture.ProviderId}/agreement", null, CancellationToken.None));
    }

    [Test]
    public async Task GetAccountUsers_VerifyProviderAgreementIsReturned()
    {
        var fixture = new WhenCallingPasAccountApiClientFixture().SetupResponseForGetAgreement();
        var result = await fixture.PasAccountApiClient.GetAgreement(fixture.ProviderId, CancellationToken.None);
        Assert.That(fixture.ProviderAgreementStatus, Is.EqualTo(result.Status));
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
    public ProviderAgreementStatus ProviderAgreementStatus { get; set; }

    public WhenCallingPasAccountApiClientFixture()
    {
        var autoFixture = new Fixture();
        MockRestHttpClient = new Mock<IRestHttpClient>();
        UserRef = autoFixture.Create<string>();
        User = autoFixture.Create<User>();
        Users = new List<User>();
        PasAccountApiClient = new PasAccountApiClient(MockRestHttpClient.Object);
        ProviderEmailRequest = autoFixture.Create<ProviderEmailRequest>();
        ProviderAgreementStatus = ProviderAgreementStatus.Agreed ;
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

    public WhenCallingPasAccountApiClientFixture SetupResponseForGetAgreement()
    {
        MockRestHttpClient.Setup(x => x.Get<ProviderAgreement>(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProviderAgreement { Status = ProviderAgreementStatus });
        return this;
    }
}