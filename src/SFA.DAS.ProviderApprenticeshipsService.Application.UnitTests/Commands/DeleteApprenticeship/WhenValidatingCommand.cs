using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteApprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.DeleteApprenticeship
{
    [TestFixture]
    public sealed class WhenValidatingCommand
    {
        private IRequestHandler<DeleteApprenticeshipCommand> _handler;
        private DeleteApprenticeshipCommand _validCommand;

        [SetUp]
        public void Setup()
        {
            _validCommand = new DeleteApprenticeshipCommand
            {
                ProviderId = 111L,
                ApprenticeshipId = 123L
            };

            _handler = new DeleteApprenticeshipCommandHandler(new DeleteApprenticeshipCommandValidator(), Mock.Of<IProviderCommitmentsApi>());
        }

        [Test]
        public void ShouldThrowExceptionIfProviderIdIsNotGreaterThanZero()
        {
            _validCommand.ProviderId = 0;

            Func<Task> act = async () => await _handler.Handle(_validCommand, new CancellationToken());

            act.ShouldThrow<ValidationException>().Which.Message.Contains("Provider Id");
        }

        [Test]
        public void ShouldThrowExceptionIfApprenticeshipIdIsNotGreaterThanZero()
        {
            _validCommand.ApprenticeshipId = 0;

            Func<Task> act = async () => await _handler.Handle(_validCommand, new CancellationToken());

            act.ShouldThrow<ValidationException>().Which.Message.Contains("Provider Id");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void ShouldThrowExceptionIfUserIdIsEmpty(string userId)
        {
            _validCommand.UserId = userId;

            Func<Task> act = async () => await _handler.Handle(_validCommand, new CancellationToken());

            act.ShouldThrow<ValidationException>().Which.Message.Contains("User Id");
        }
    }
}
