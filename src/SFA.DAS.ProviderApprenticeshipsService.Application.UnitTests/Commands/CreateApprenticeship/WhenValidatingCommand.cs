using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateApprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.CreateApprenticeship
{
    public sealed class WhenValidatingCommand
    {
        private CreateApprenticeshipCommandValidator _validator;
        private CreateApprenticeshipCommand _exampleValidCommand;

        [SetUp]
        public void Setup()
        {
            _validator = new CreateApprenticeshipCommandValidator();
            var exampleValidApprenticeship = new Apprenticeship { CommitmentId = 1234 };

            _exampleValidCommand = new CreateApprenticeshipCommand
            {
                UserId = "user1234",
                ProviderId = 111L,
                Apprenticeship = exampleValidApprenticeship
            };
        }

        [Test]
        public void ThenACorrectlyPopulatedCommandReturnsValid()
        {
            var result = _validator.Validate(_exampleValidCommand);

            result.IsValid.Should().BeTrue();
        }

        [TestCase(0)]
        [TestCase(-2)]
        public void ThenProviderIdsLessThanOneIsInvalid(long providerId)
        {
            _exampleValidCommand.ProviderId = providerId;

            var result = _validator.Validate(_exampleValidCommand);

            result.IsValid.Should().BeFalse();
        }

        [TestCase(0)]
        [TestCase(-2)]
        public void ThenCommitmentIdIsLessThanOneIsInvalid(long commitmentId)
        {
            _exampleValidCommand.Apprenticeship.CommitmentId = commitmentId;

            var result = _validator.Validate(_exampleValidCommand);

            result.IsValid.Should().BeFalse();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void ThenAnEmptyUserIdIsInvalid(string userId)
        {
            _exampleValidCommand.UserId = userId;

            var result = _validator.Validate(_exampleValidCommand);

            result.IsValid.Should().BeFalse();
        }
    }
}
