using System.Threading.Tasks;

using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.Tasks.Api.Client;


using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.SubmitCommitment
{
    [TestFixture]
    public sealed class WhenApprovingAsFirstApprover
    {
        private SubmitCommitmentCommand _validCommand;
        private Mock<IProviderCommitmentsApi> _mockCommitmentsApi;
        private Mock<IMediator> _mockMediator;
        private Mock<ITasksApi> _mockTasksApi;
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
                .ReturnsAsync(new Commitment
                {
                    ProviderId = _validCommand.ProviderId,
                    AgreementStatus = AgreementStatus.NotAgreed,
                    Reference = "ABC123",
                    EmployerLastUpdateInfo = new LastUpdateInfo
                    {
                        EmailAddress = "EmployerTestEmail"
                    }
                });

            var configuration = new ProviderApprenticeshipsServiceConfiguration { EnableEmailNotifications = true };
            _mockTasksApi = new Mock<ITasksApi>();
            _mockMediator = new Mock<IMediator>();
            _handler = new SubmitCommitmentCommandHandler(_mockCommitmentsApi.Object, _mockTasksApi.Object, new SubmitCommitmentCommandValidator(), _mockMediator.Object, configuration);
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
            arg.Email.TemplateId.Should().Be("EmployerCommitmentNotification");
            arg.Email.Tokens["type"].Should().Be("approval");
            arg.Email.Tokens["cohort_reference"].Should().Be("ABC123");
        }
    }
}
