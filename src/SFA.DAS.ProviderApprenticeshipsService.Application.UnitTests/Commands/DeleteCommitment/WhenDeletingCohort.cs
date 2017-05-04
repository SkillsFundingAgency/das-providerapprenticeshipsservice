using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteCommitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.DeleteCommitment
{
    [TestFixture]
    public sealed class WhenDeletingCommitment
    {
        private DeleteCommitmentCommandHandler _handler;
        private DeleteCommitmentCommand _validCommand;
        private Mock<IProviderCommitmentsApi> _mockCommitmentsApi;

        [SetUp]
        public void Setup()
        {
            _validCommand = new DeleteCommitmentCommand
            {
                UserId = "user123",
                ProviderId = 111L,
                CommitmentId = 123L,
                UserDisplayName = "Bob",
                UserEmailAddress = "test@email.com"
            };

            _mockCommitmentsApi = new Mock<IProviderCommitmentsApi>();
            _handler = new DeleteCommitmentCommandHandler(new DeleteCommitmentCommandValidator(), _mockCommitmentsApi.Object);
        }

        [Test]
        public async Task ShouldCallCommitmentsApi()
        {
            await _handler.Handle(_validCommand);

            _mockCommitmentsApi.Verify(
                x =>
                    x.DeleteProviderCommitment(_validCommand.ProviderId, _validCommand.CommitmentId,
                        It.Is<DeleteRequest>(
                            r => r.UserId == _validCommand.UserId && r.LastUpdatedByInfo.EmailAddress == _validCommand.UserEmailAddress && r.LastUpdatedByInfo.Name == _validCommand.UserDisplayName)));
        }
    }
}
