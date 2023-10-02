using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.PAS.Account.Application.Commands.SendNotification;

namespace SFA.DAS.PAS.Account.Api.UnitTests.Orchestrator;

[TestFixture]
public class WhenSendingEmailToAllProviderRecipients_UserTypes
{
    private Mock<IAccountOrchestrator> _accountOrchestrator;
    private Mock<IMediator> _mediator;
    private long _ukprn;
    private ProviderEmailRequest _request;
    private string _templateId;
    private Dictionary<string, string> _tokens;
    private User _superUser;
    private User _normalUserWithNotificationsEnabled;
    private User _normalUserWithNotificationsDisabled;
    private List<User> _accountUsers;

    [SetUp]
    public void Setup()
    {
        _ukprn = 228987165;
        _templateId = Guid.NewGuid().ToString();
        _tokens = new Dictionary<string, string>
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };

        _normalUserWithNotificationsEnabled = new User { EmailAddress = "normal@test.com", IsSuperUser = false, ReceiveNotifications = true };
        _normalUserWithNotificationsDisabled = new User { EmailAddress = "normal@test.com", IsSuperUser = false, ReceiveNotifications = false };
        _superUser = new User { EmailAddress = "super@test.com", IsSuperUser = true, ReceiveNotifications = true };

        _accountUsers = new List<User>
        {
            _normalUserWithNotificationsEnabled,
            _superUser
        };

        _accountOrchestrator = new Mock<IAccountOrchestrator>();
        _mediator = new Mock<IMediator>();

        _accountOrchestrator
            .Setup(x => x.GetAccountUsers(_ukprn))
            .ReturnsAsync(_accountUsers);

        _request = new ProviderEmailRequest
        {
            TemplateId = _templateId,
            Tokens = _tokens
        };
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Should_Only_Send_Notifications_To_NormaUsers_With_ReceiveNotifications_Enabled(bool explicitListIsNull)
    {
        _request.ExplicitEmailAddresses = explicitListIsNull ? null : new List<string>();

        var sut = new EmailOrchestrator(_accountOrchestrator.Object, _mediator.Object, Mock.Of<ILogger<EmailOrchestrator>>());
        await sut.SendEmailToAllProviderRecipients(_ukprn, _request);

        _mediator.Verify(x => x.Send(It.Is<SendNotificationCommand>(c => c.Email.RecipientsAddress == "normal@test.com"), It.IsAny<CancellationToken>()), Times.Once);
        _mediator.Verify(x => x.Send(It.Is<SendNotificationCommand>(c => c.Email.RecipientsAddress == "super@test.com"), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task Should_Send_Notifications_To_SuperUsers_When_There_Are_No_NormalUsers(bool explicitListIsNull)
    {
        _request.ExplicitEmailAddresses = explicitListIsNull ? null : new List<string>();
        _accountUsers.Remove(_normalUserWithNotificationsEnabled);

        var sut = new EmailOrchestrator(_accountOrchestrator.Object, _mediator.Object, Mock.Of<ILogger<EmailOrchestrator>>());
        await sut.SendEmailToAllProviderRecipients(_ukprn, _request);

        _mediator.Verify(x => x.Send(It.Is<SendNotificationCommand>(c => c.Email.RecipientsAddress == "normal@test.com"), It.IsAny<CancellationToken>()), Times.Never);
        _mediator.Verify(x => x.Send(It.Is<SendNotificationCommand>(c => c.Email.RecipientsAddress == "super@test.com"), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public async Task Should_Send_Notification_To_SuperUsers_When_There_Are_No_NormalUsers_With_Notifications_Enabled(bool explicitListIsNull)
    {
        _request.ExplicitEmailAddresses = explicitListIsNull ? null : new List<string>();
        _accountUsers.Remove(_normalUserWithNotificationsEnabled);
        _accountUsers.Add(_normalUserWithNotificationsDisabled);

        var sut = new EmailOrchestrator(_accountOrchestrator.Object, _mediator.Object, Mock.Of<ILogger<EmailOrchestrator>>());
        await sut.SendEmailToAllProviderRecipients(_ukprn, _request);

        _mediator.Verify(x => x.Send(It.Is<SendNotificationCommand>(c => c.Email.RecipientsAddress == "normal@test.com"), It.IsAny<CancellationToken>()), Times.Never);
        _mediator.Verify(x => x.Send(It.Is<SendNotificationCommand>(c => c.Email.RecipientsAddress == "super@test.com"), It.IsAny<CancellationToken>()), Times.Once);
    }
}