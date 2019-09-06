using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;


using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.HashingService;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.SubmitCommitment
{
    [TestFixture]
    public sealed class WhenApprovingAsFirstApprover
    {
        private SubmitCommitmentCommand _validCommand;
        private Mock<IProviderCommitmentsApi> _mockCommitmentsApi;
        private Mock<IMediator> _mockMediator;
        private Mock<IHashingService> _mockHashingService;
        private IRequestHandler<SubmitCommitmentCommand> _handler;

        [SetUp]
        public void Setup()
        {
            _validCommand = new SubmitCommitmentCommand()
            {
                ProviderId = 111L,
                HashedCommitmentId = "ABC123",
                CommitmentId = 123L,
                Message = "Test Message",
                CreateTask = true,
                LastAction = LastAction.Approve,
                UserDisplayName = "Test User",
                UserEmailAddress = "Test@test.com",
                UserId = "user123"
            };

            _mockCommitmentsApi = new Mock<IProviderCommitmentsApi>();
            _mockCommitmentsApi.Setup(x => x.GetProviderCommitment(_validCommand.ProviderId, _validCommand.CommitmentId))
                .ReturnsAsync(new CommitmentView
                {
                    ProviderId = _validCommand.ProviderId,
                    AgreementStatus = AgreementStatus.NotAgreed,
                    Reference = "ABC123",
                    EmployerLastUpdateInfo = new LastUpdateInfo
                    {
                        EmailAddress = "EmployerTestEmail"
                    },
                    ProviderName = "ProviderName",
                    EmployerAccountId = 100
                });

            _mockHashingService = new Mock<IHashingService>();
            _mockHashingService.Setup(x => x.HashValue(It.IsAny<long>())).Returns<long>((p) => "HS"+p.ToString());

            var configuration = new ProviderApprenticeshipsServiceConfiguration { EnableEmailNotifications = true };
            _mockMediator = new Mock<IMediator>();
            _handler = new SubmitCommitmentCommandHandler(_mockCommitmentsApi.Object,
                new SubmitCommitmentCommandValidator(),
                _mockMediator.Object,
                configuration,
                _mockHashingService.Object);
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
        public async Task ShouldSendRequestToApproveToEmployer()
        {
            SendNotificationCommand arg = null;

            _mockMediator.Setup(x => x.Send(It.IsAny<SendNotificationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Unit())
                .Callback<SendNotificationCommand, CancellationToken>((command, token) => arg = command);

            await _handler.Handle(_validCommand, new CancellationToken());

            _mockMediator.Verify(x => x.Send(It.IsAny<SendNotificationCommand>(), It.IsAny<CancellationToken>()), Times.Once);

            arg.Email.RecipientsAddress.Should().Be("EmployerTestEmail");
            arg.Email.TemplateId.Should().Be("EmployerCohortNotification");
            arg.Email.Tokens["cohort_reference"].Should().Be("ABC123");
            arg.Email.Tokens["provider_name"].Should().Be("ProviderName");
            arg.Email.Tokens["employer_hashed_account"].Should().Be("HS100");
        }

        [Test]
        public async Task ShouldSendNotRequestToApproveToEmployerIfEmailAddressUnknown()
        {
            _mockCommitmentsApi.Setup(x => x.GetProviderCommitment(_validCommand.ProviderId, _validCommand.CommitmentId))
                .ReturnsAsync(new CommitmentView
                {
                    ProviderId = _validCommand.ProviderId,
                    AgreementStatus = AgreementStatus.NotAgreed,
                    Reference = "ABC123",
                    EmployerLastUpdateInfo = new LastUpdateInfo
                    {
                        EmailAddress = ""
                    }
                });

            await _handler.Handle(_validCommand, new CancellationToken());

            _mockMediator.Verify(x => x.Send(It.IsAny<SendNotificationCommand>(), It.IsAny<CancellationToken>()), Times.Never);

        }
    }
}
