using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.CreateApprenticeship
{
    [TestFixture]
    public sealed class WhenCreatingAnApprentice
    {
        private Mock<IProviderCommitmentsApi> _mockCommitmentsApi;
        private CreateApprenticeshipCommand _exampleValidCommand;
        private CreateApprenticeshipCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _mockCommitmentsApi = new Mock<IProviderCommitmentsApi>();
            _exampleValidCommand = new CreateApprenticeshipCommand
            {
                UserId = "user123",
                ProviderId = 123L,
                Apprenticeship = new Apprenticeship { CommitmentId = 123 },
                UserEmailAddress = "test@email.com",
                UserDisplayName = "Bob"
            };

            _handler = new CreateApprenticeshipCommandHandler(_mockCommitmentsApi.Object);
        }

        [Test]
        public async Task ShouldCallCommitmentApi()
        {
            await _handler.Handle(_exampleValidCommand);

            _mockCommitmentsApi.Verify(
                x =>
                    x.CreateProviderApprenticeship(_exampleValidCommand.ProviderId, _exampleValidCommand.Apprenticeship.CommitmentId,
                        It.Is<ApprenticeshipRequest>(
                            r =>
                                r.Apprenticeship == _exampleValidCommand.Apprenticeship && 
                                r.UserId == _exampleValidCommand.UserId &&
                                r.LastUpdatedByInfo.EmailAddress == _exampleValidCommand.UserEmailAddress &&
                                r.LastUpdatedByInfo.Name == _exampleValidCommand.UserDisplayName)));
        }

        [Test]
        public void ShouldThrowAnExceptionOnValidationFailure()
        {
            _exampleValidCommand.ProviderId = 0; // This is invalid

            Func<Task> act = async () => { await _handler.Handle(_exampleValidCommand); };

            act.ShouldThrow<InvalidRequestException>();
        }
    }
}
