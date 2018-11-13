using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.HashingService;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.SubmitCommitment
{
    [TestFixture]
    public sealed class WhenApprovingAsSecondApprover
    {
        private CommitmentView _commitmentView;
        private SubmitCommitmentCommand _validCommand;
        private Mock<IProviderCommitmentsApi> _mockCommitmentsApi;
        private Mock<IMediator> _mockMediator;
        private Mock<IHashingService> _mockHashingService;
        private IRequestHandler<SubmitCommitmentCommand> _handler;

        private const string CohortReference = "ABC123";
        private const string UserName = "Anita Bush";
        private const long EmployerAccountId = 54321L;
        private const string HashedEmployerAccountId = "HSHCK";

        [SetUp]
        public void Setup()
        {
            _validCommand = new SubmitCommitmentCommand()
            {
                ProviderId = 111L,
                HashedCommitmentId = CohortReference,
                CommitmentId = 123L,
                Message = "Test Message",
                CreateTask = true,
                LastAction = LastAction.Approve,
                UserDisplayName = UserName,
                UserEmailAddress = "Test@test.com",
                UserId = "user123"
            };

            _commitmentView = new CommitmentView
            {
                ProviderId = _validCommand.ProviderId,
                AgreementStatus = AgreementStatus.EmployerAgreed,
                Reference = CohortReference,
                EmployerLastUpdateInfo = new LastUpdateInfo
                {
                    EmailAddress = "EmployerTestEmail"
                },
                EmployerAccountId = EmployerAccountId
            };

            _mockCommitmentsApi = new Mock<IProviderCommitmentsApi>();
            _mockCommitmentsApi.Setup(x => x.GetProviderCommitment(_validCommand.ProviderId, _validCommand.CommitmentId))
                .ReturnsAsync(_commitmentView);

            _mockHashingService = new Mock<IHashingService>();
            _mockHashingService.Setup(x => x.HashValue(EmployerAccountId)).Returns(HashedEmployerAccountId);

            var configuration = new ProviderApprenticeshipsServiceConfiguration { EnableEmailNotifications = true };
            _mockMediator = new Mock<IMediator>();
            _handler = new SubmitCommitmentCommandHandler(_mockCommitmentsApi.Object, new SubmitCommitmentCommandValidator(), _mockMediator.Object, configuration, _mockHashingService.Object);
        }

        [Test]
        public async Task ShouldApproveTheCohort()
        {
            await _handler.Handle(_validCommand, new CancellationToken());

            _mockCommitmentsApi.Verify(x => x.ApproveCohort(_validCommand.ProviderId, _validCommand.CommitmentId,
                It.Is<CommitmentSubmission>(y =>
                    y.Message == _validCommand.Message && y.UserId == _validCommand.UserId && y.LastUpdatedByInfo.EmailAddress == _validCommand.UserEmailAddress &&
                    y.LastUpdatedByInfo.Name == _validCommand.UserDisplayName)));
        }

        [Test]
        public async Task ShouldSendNotificationOfApprovalToEmployer()
        {
            SendNotificationCommand arg = null;

            _mockMediator.Setup(x => x.Send(It.IsAny<SendNotificationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Unit())
                .Callback<SendNotificationCommand, CancellationToken>((command, token) => arg = command);


            await _handler.Handle(_validCommand, new CancellationToken());

            _mockMediator.Verify(x => x.Send(It.IsAny<SendNotificationCommand>(), It.IsAny<CancellationToken>()), Times.Once);

            arg.Email.RecipientsAddress.Should().Be("EmployerTestEmail");
            arg.Email.TemplateId.Should().Be("EmployerCohortApproved");
            arg.Email.Tokens["type"].Should().Be("approval");
            arg.Email.Tokens["cohort_reference"].Should().Be(CohortReference);
        }

        [Test]
        public async Task AndTransferFundedShouldSendNotificationOfApprovalToEmployer()
        {
            const string senderName = "Sender";
            const string providerName = "Provider";

            const string template =
                @"Dear ((first_name)),

((provider_name)) has approved cohort ((cohort_reference)).

What happens next?
A transfer request has been sent to ((sender_name)) for approval. You will receive a notification once ((sender_name)) approves or rejects the request.

To view cohort ((cohort_reference)) and its progress, follow the link below.
https://manage-apprenticeships.service.gov.uk/commitments/accounts/((employer_hashed_account))/apprentices/cohorts

This is an automated message. Please do not reply to this email.

Kind regards,

Apprenticeship service team";

            string expectedEmailBody = $@"Dear {UserName},

{providerName} has approved cohort {CohortReference}.

What happens next?
A transfer request has been sent to {senderName} for approval. You will receive a notification once {senderName} approves or rejects the request.

To view cohort {CohortReference} and its progress, follow the link below.
https://manage-apprenticeships.service.gov.uk/commitments/accounts/{HashedEmployerAccountId}/apprentices/cohorts

This is an automated message. Please do not reply to this email.

Kind regards,

Apprenticeship service team";

            _commitmentView.TransferSender = new TransferSender
                { Name = senderName };
            _commitmentView.ProviderName = providerName;

            SendNotificationCommand arg = null;
            _mockMediator.Setup(x => x.Send(It.IsAny<SendNotificationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Unit())
                .Callback<SendNotificationCommand, CancellationToken>((command, token) => arg = command);

            await _handler.Handle(_validCommand, new CancellationToken());

            _mockMediator.Verify(x => x.Send(It.IsAny<SendNotificationCommand>(), It.IsAny<CancellationToken>()), Times.Once);

            arg.Email.RecipientsAddress.Should().Be("EmployerTestEmail");
            arg.Email.TemplateId.Should().Be("EmployerTransferPendingFinalApproval");
            arg.Email.Tokens["first_name"].Should().Be(UserName);
            arg.Email.Tokens["cohort_reference"].Should().Be(CohortReference);
            arg.Email.Tokens["sender_name"].Should().Be(senderName);
            arg.Email.Tokens["provider_name"].Should().Be(providerName);
            arg.Email.Tokens["employer_hashed_account"].Should().Be(HashedEmployerAccountId);

            var emailBody = PopulateTemplate(template, arg.Email.Tokens);
            TestContext.WriteLine(emailBody);
            Assert.AreEqual(expectedEmailBody, emailBody);
        }

        private string PopulateTemplate(string template, Dictionary<string, string> tokens)
        {
            foreach (var token in tokens)
            {
                template = template.Replace($"(({token.Key}))", token.Value);
            }

            return template;
        }
    }
}
