using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.Account.Api.UnitTests.Orchestrator
{
    [TestFixture]
    public class WhenSendingEmailToAllProviderRecipients_UserTypes
    {
        private EmailOrchestrator _sut;
        private Mock<IAccountOrchestrator> _accountOrchestrator;
        private Mock<IMediator> _mediator;
        private long _ukprn;
        private ProviderEmailRequest _request;
        private string _templateId;
        private Dictionary<string, string> _tokens;
        private User _superUser;
        private User _normalUser;
        private List<User> _accountUsers;

        [SetUp]
        public void Setup()
        {
            _ukprn = 228987165;
            _templateId = Guid.NewGuid().ToString();
            _tokens = new Dictionary<string, string>();
            _tokens.Add("key1", "value1");
            _tokens.Add("key2", "value2");

            _normalUser = new User { EmailAddress = "normal@test.com", IsSuperUser = false, ReceiveNotifications = true };
            _superUser = new User { EmailAddress = "super@test.com", IsSuperUser = true, ReceiveNotifications = true };
            
            _accountUsers = new List<User>();
            _accountUsers.Add(_normalUser);
            _accountUsers.Add(_superUser);

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

        [Test]
        public async Task ShouldOnlySendNotificationForNormaUser()
        {

            _sut = new EmailOrchestrator(_accountOrchestrator.Object, _mediator.Object, Mock.Of<IProviderCommitmentsLogger>());
            await _sut.SendEmailToAllProviderRecipients(_ukprn, _request);

            _mediator.Verify(x => x.Send(It.Is<SendNotificationCommand>(c=>c.Email.RecipientsAddress == "normal@test.com"), It.IsAny<CancellationToken>()), Times.Once);
            _mediator.Verify(x => x.Send(It.Is<SendNotificationCommand>(c => c.Email.RecipientsAddress == "super@test.com"), It.IsAny<CancellationToken>()), Times.Never);
        }
        [Test]
        public async Task ShouldOnlySendNotificationForSuperUser()
        {
            _accountUsers.Remove(_normalUser);

            _sut = new EmailOrchestrator(_accountOrchestrator.Object, _mediator.Object, Mock.Of<IProviderCommitmentsLogger>());
            await _sut.SendEmailToAllProviderRecipients(_ukprn, _request);

            _mediator.Verify(x => x.Send(It.Is<SendNotificationCommand>(c => c.Email.RecipientsAddress == "normal@test.com"), It.IsAny<CancellationToken>()), Times.Never);
            _mediator.Verify(x => x.Send(It.Is<SendNotificationCommand>(c => c.Email.RecipientsAddress == "super@test.com"), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}