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
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteRegisteredUser;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;

namespace SFA.DAS.PAS.Account.Api.UnitTests.Orchestrator
{
    [TestFixture]
    public class WhenSendingEmailToAllProviderRecipients_IdamsError
    {
        private EmailOrchestrator _sut;
        private Mock<IAccountOrchestrator> _accountOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IIdamsEmailServiceWrapper> _idamsEmailServiceWrapper;
        private long _ukprn;
        private List<string> _emailAddresses;
        private List<string> _idamsUsers;
        private ProviderEmailRequest _request;
        private string _templateId;
        private Dictionary<string, string> _tokens;

        [SetUp]
        public async Task Setup()
        {
            _ukprn = 228987165;
            _emailAddresses = new List<string>
            {
                "test1@example.com",
                "test2@example.com",
                "nobody@idams.com"
            };
            _templateId = Guid.NewGuid().ToString();
            _tokens = new Dictionary<string, string>();
            _tokens.Add("key1", "value1");
            _tokens.Add("key2", "value2");

            _accountOrchestrator = new Mock<IAccountOrchestrator>();
            _mediator = new Mock<IMediator>();
            _idamsEmailServiceWrapper = new Mock<IIdamsEmailServiceWrapper>();

            _accountOrchestrator
                .Setup(x => x.GetAccountUsers(_ukprn))
                .ReturnsAsync(_emailAddresses.Select(x => new User { UserRef = $"ref-{x}", EmailAddress = x, ReceiveNotifications = true }));

            _idamsEmailServiceWrapper
                .Setup(x => x.GetEmailsAsync(It.IsAny<long>()))
                .Throws<ArgumentException>();

            _idamsEmailServiceWrapper
                .Setup(x => x.GetSuperUserEmailsAsync(It.IsAny<long>()))
                .Throws<ArgumentException>();

            _request = new ProviderEmailRequest
            {
                TemplateId = _templateId,
                Tokens = _tokens
            };

            _sut = new EmailOrchestrator(_accountOrchestrator.Object, _mediator.Object, _idamsEmailServiceWrapper.Object, Mock.Of<IProviderCommitmentsLogger>());
            await _sut.SendEmailToAllProviderRecipients(_ukprn, _request);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void ShouldSendNotificationToEachAddress(int index)
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

        [Test]
        public void ShouldNotDeleteAnyUserAccountsNotFoundInIdams()
        {
            _mediator.Verify(x => x.Send(It.IsAny<DeleteRegisteredUserCommand>(),It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
