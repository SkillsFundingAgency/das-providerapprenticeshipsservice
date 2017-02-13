using System;
using System.Threading.Tasks;

using FluentAssertions;

using FluentValidation;

using Moq;

using NUnit.Framework;

using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteCommitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.DeleteCommitment
{
    [TestFixture]
    public class WhenValidatingDeletingCommitmentCommand
    {
        private DeleteCommitmentCommand _validCommand;

        private Mock<ICommitmentsApi> _mockCommitmentsApi;

        private DeleteCommitmentCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _validCommand = new DeleteCommitmentCommand()
            {
                ProviderId = 111L,
                CommitmentId = 123L
            };

            _mockCommitmentsApi = new Mock<ICommitmentsApi>();
            _handler = new DeleteCommitmentCommandHandler(new DeleteCommitmentCommandValidator(), _mockCommitmentsApi.Object);
        }

        [Test]
        public void ShouldNotThrowExceptionIfProviderIdAndCommitmentid()
        {
            Func<Task> act = async () => await _handler.Handle(_validCommand);

            act.ShouldNotThrow<ValidationException>();
        }

        [Test]
        public void ShouldThrowExceptionIfProviderIdIsNotGreaterThanZero()
        {
            _validCommand.ProviderId = 0;

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            var m = act.ShouldThrow<ValidationException>().Which.Message;
            m.Should().Contain("Provider Id");
            m.Should().NotContain("Commitment Id");
        }

        [Test]
        public void ShouldThrowExceptionIfApprenticeshipIdIsNotGreaterThanZero()
        {
            _validCommand.CommitmentId = 0;

            Func<Task> act = async () => await _handler.Handle(_validCommand);

            var m = act.ShouldThrow<ValidationException>().Which.Message;
            m.Should().NotContain("Provider Id");
            m.Should().Contain("Commitment Id");
        }
    }
}
