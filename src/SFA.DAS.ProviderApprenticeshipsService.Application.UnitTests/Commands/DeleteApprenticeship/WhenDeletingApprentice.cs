﻿using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteApprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.DeleteApprenticeship
{
    [TestFixture]
    public sealed class WhenDeletingApprentice
    {
        private DeleteApprenticeshipCommandHandler _handler;
        private DeleteApprenticeshipCommand _validCommand;
        private Mock<IProviderCommitmentsApi> _mockCommitmentsApi;

        [SetUp]
        public void Setup()
        {
            _validCommand = new DeleteApprenticeshipCommand
            {
                UserId = "user123",
                ProviderId = 111L,
                ApprenticeshipId = 123L
            };

            _mockCommitmentsApi = new Mock<IProviderCommitmentsApi>();
            _handler = new DeleteApprenticeshipCommandHandler(new DeleteApprenticeshipCommandValidator(), _mockCommitmentsApi.Object);
        }

        [Test]
        public async Task ShouldCallCommitmentsApi()
        {
            await _handler.Handle(_validCommand);

            _mockCommitmentsApi.Verify(x => x.DeleteProviderApprenticeship(
                It.Is<long>(a => a == _validCommand.ProviderId), It.Is<long>(a => a ==_validCommand.ApprenticeshipId), It.Is<DeleteRequest>(a => a.UserId == _validCommand.UserId)));
        }
    }
}
