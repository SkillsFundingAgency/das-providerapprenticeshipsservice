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

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.SubmitCommitment
{
    [TestFixture]
    public sealed class WhenApprovingAsSecondApprover
    {
        private SubmitCommitmentCommand _validCommand;
        private Mock<IProviderCommitmentsApi> _mockCommitmentsApi;
        private Mock<IMediator> _mockMediator;
        private SubmitCommitmentCommandHandler _handler;

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
                UserId = "user123",
            };

            _mockCommitmentsApi = new Mock<IProviderCommitmentsApi>();
            _mockCommitmentsApi.Setup(x => x.GetProviderCommitment(_validCommand.ProviderId, _validCommand.CommitmentId))
                .ReturnsAsync(new CommitmentView
                {
                    ProviderId = _validCommand.ProviderId,
                    AgreementStatus = AgreementStatus.EmployerAgreed,
                    Reference = "ABC123",
                    EmployerLastUpdateInfo = new LastUpdateInfo
                    {
                        EmailAddress = "EmployerTestEmail"
                    }
                });

            var configuration = new ProviderApprenticeshipsServiceConfiguration { EnableEmailNotifications = true };
            _mockMediator = new Mock<IMediator>();
            _handler = new SubmitCommitmentCommandHandler(_mockCommitmentsApi.Object, new SubmitCommitmentCommandValidator(), _mockMediator.Object, configuration);
        }

        [Test]
        public async Task ShouldSendRequestToApproveToEmployer()
        {
            SendNotificationCommand arg = null;

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<SendNotificationCommand>()))
                .ReturnsAsync(new Unit()).Callback<SendNotificationCommand>(x => arg = x);

            await _handler.Handle(_validCommand);

            _mockMediator.Verify(x => x.SendAsync(It.IsAny<SendNotificationCommand>()), Times.Once);

            arg.Email.RecipientsAddress.Should().Be("EmployerTestEmail");
            arg.Email.TemplateId.Should().Be("EmployerCohortApproved");
            arg.Email.Tokens["type"].Should().Be("approval");
            arg.Email.Tokens["cohort_reference"].Should().Be("ABC123");
        }
    }
}
