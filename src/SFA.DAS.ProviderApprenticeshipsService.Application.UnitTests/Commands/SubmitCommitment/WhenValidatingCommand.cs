using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.SubmitCommitment;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.Tasks.Api.Client;
using System;
using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.SubmitCommitment
{
    [TestFixture]
    public sealed class WhenValidatingCommand
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
                LastAction = LastAction.Amend,
                UserDisplayName = "Test User",
                UserEmailAddress = "Test@test.com",
                UserId = "user123",
            };

            _mockCommitmentsApi = new Mock<IProviderCommitmentsApi>();
            _mockTasksApi = new Mock<ITasksApi>();
            _mockMediator = new Mock<IMediator>();
            _handler = new SubmitCommitmentCommandHandler(_mockCommitmentsApi.Object, _mockTasksApi.Object, new SubmitCommitmentCommandValidator(), _mockMediator.Object, new ProviderApprenticeshipsServiceConfiguration());
        }

        [Test]
        public void ShouldThrowExceptionIfProviderIdIsNotGreaterThanZero()
        {
            _validCommand.ProviderId = 0;

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            var m = act.ShouldThrow<InvalidRequestException>();
            m.Which.ErrorMessages.Count.Should().Be(1);
            m.Which.ErrorMessages.ContainsKey("ProviderId");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ShouldThrowExceptionIfHashedCommitmentIdIsEmpty(string hashedCommitmentId)
        {
            _validCommand.HashedCommitmentId = hashedCommitmentId;

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            var m = act.ShouldThrow<InvalidRequestException>();
            m.Which.ErrorMessages.Count.Should().Be(1);
            m.Which.ErrorMessages.ContainsKey("HashedCommitmentid");
        }

        [Test]
        public void ShouldThrowExceptionIfCommitmentIdIsNotGreaterThanZero()
        {
            _validCommand.CommitmentId = 0;

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            var m = act.ShouldThrow<InvalidRequestException>();
            m.Which.ErrorMessages.Count.Should().Be(1);
            m.Which.ErrorMessages.ContainsKey("CommitmentId");
        }

        [Test]
        public void ShouldThrowExceptionIfLastActionIsNone()
        {
            _validCommand.LastAction = LastAction.None;

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            var m = act.ShouldThrow<InvalidRequestException>();
            m.Which.ErrorMessages.Count.Should().Be(1);
            m.Which.ErrorMessages.ContainsKey("LastAction");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ShouldThrowExceptionIfUserDisplayNameIsEmpty(string userDisplayName)
        {
            _validCommand.UserDisplayName = userDisplayName;

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            var m = act.ShouldThrow<InvalidRequestException>();
            m.Which.ErrorMessages.Count.Should().Be(1);
            m.Which.ErrorMessages.ContainsKey("UserDisplayName");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ShouldThrowExceptionIfUserEmailAddressIsEmpty(string userEmailAddress)
        {
            _validCommand.UserEmailAddress = userEmailAddress;

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            var m = act.ShouldThrow<InvalidRequestException>();
            m.Which.ErrorMessages.Count.Should().Be(1);
            m.Which.ErrorMessages.ContainsKey("UserEmailAddress");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ShouldThrowExceptionIfUserIdIsEmpty(string userId)
        {
            _validCommand.UserId = userId;

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            var m = act.ShouldThrow<InvalidRequestException>();
            m.Which.ErrorMessages.Count.Should().Be(1);
            m.Which.ErrorMessages.ContainsKey("UserId");
        }
    }
}
