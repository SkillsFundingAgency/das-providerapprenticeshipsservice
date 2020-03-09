using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.PAS.Account.Api.Orchestrator;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

namespace SFA.DAS.PAS.Account.Api.UnitTests.Orchestrator
{
    [TestFixture]
    public class WhenSendingEmailToAllProviderRecipients_IdamsSuperUserEmails
    {
        private EmailOrchestrator _sut;
        private Mock<IAccountOrchestrator> _accountOrchestrator;
        private List<User> _accountUsers;
        private Mock<IMediator> _mediator;
        private Mock<IIdamsEmailServiceWrapper> _idamsEmailServiceWrapper;
        private long _ukprn;
        private List<string> _emailAddresses;
        private ProviderEmailRequest _request;
        private string _templateId;
        private Dictionary<string, string> _tokens;
        private ProviderApprenticeshipsServiceConfiguration _configuration;

        [SetUp]
        public async Task Setup()
        {
            _ukprn = 228987165;
            _emailAddresses = new List<string>
            {
                "test1@example.com",
                "test2@example.com",
                "test3@example.com",
                "test4@example.com"
            };
            _templateId = Guid.NewGuid().ToString();
            _tokens = new Dictionary<string, string>();
            _tokens.Add("key1", "value1");
            _tokens.Add("key2", "value2");
            _accountUsers = new List<User>
            {
                new User
                {
                    EmailAddress = "test3@example.com",
                    ReceiveNotifications = false
                },
                new User
                {
                    EmailAddress = "test4@example.com",
                    ReceiveNotifications = true
                }
            };

            _accountOrchestrator = new Mock<IAccountOrchestrator>();
            _accountOrchestrator.Setup(x => x.GetAccountUsers(It.IsAny<long>())).ReturnsAsync(_accountUsers);

            _mediator = new Mock<IMediator>();
            _idamsEmailServiceWrapper = new Mock<IIdamsEmailServiceWrapper>();
            _configuration = new ProviderApprenticeshipsServiceConfiguration { CommitmentNotification = new ProviderNotificationConfiguration { UseProviderEmail = true } };

            _idamsEmailServiceWrapper
                .Setup(x => x.GetEmailsAsync(It.IsAny<long>()))
                .ReturnsAsync(new List<string>());

            _idamsEmailServiceWrapper
                .Setup(x => x.GetSuperUserEmailsAsync(It.IsAny<long>()))
                .ReturnsAsync(_emailAddresses);

            _request = new ProviderEmailRequest
            {
                TemplateId = _templateId,
                Tokens = _tokens
            };

            _sut = new EmailOrchestrator(_accountOrchestrator.Object, _mediator.Object, _idamsEmailServiceWrapper.Object, _configuration);
            await _sut.SendEmailToAllProviderRecipients(_ukprn, _request);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(3)]
        public void ShouldSendCommandToEachAddress(int index)
        {
            _mediator.Verify(x => x.Send(It.Is<SendNotificationCommand>(y
                => y.Email.RecipientsAddress == _emailAddresses[index]
                   && y.Email.TemplateId == _templateId
                   && y.Email.Tokens.SequenceEqual(_tokens)
                   && y.Email.ReplyToAddress == "noreply@sfa.gov.uk"
                   && y.Email.Subject == "x"
                   && y.Email.SystemId == "x"
            ), It.IsAny<CancellationToken>()));
        }

        [TestCase(2)]
        public void ShouldNotSendNotificationToEachAddress(int index)
        {
            _mediator.Verify(x => x.Send(It.Is<SendNotificationCommand>(y
                => y.Email.RecipientsAddress == _emailAddresses[index]
            ), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}