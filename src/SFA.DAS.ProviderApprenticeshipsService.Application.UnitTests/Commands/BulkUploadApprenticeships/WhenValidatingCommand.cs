using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.BulkUploadApprenticeships
{
    public sealed class WhenValidatingCommand
    {
        private BulkUploadApprenticeshipsCommandValidator _validator;
        private BulkUploadApprenticeshipsCommand _exampleValidCommand;

        [SetUp]
        public void Setup()
        {
            _validator = new BulkUploadApprenticeshipsCommandValidator();
            var exampleValidApprenticeships = new List<Apprenticeship>
            {
                new Apprenticeship { FirstName = "Bob", LastName = "Smith" },
                new Apprenticeship { FirstName = "Bill", LastName = "Jones" }
            };

            _exampleValidCommand = new BulkUploadApprenticeshipsCommand
            {
                UserId = "user1234",
                ProviderId = 111L,
                CommitmentId = 123L,
                Apprenticeships = exampleValidApprenticeships
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
            _exampleValidCommand.CommitmentId = commitmentId;

            var result = _validator.Validate(_exampleValidCommand);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ThenANullListOfApprenticesIsInvalid()
        {
            _exampleValidCommand.Apprenticeships = null;

            var result = _validator.Validate(_exampleValidCommand);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ThenAnEmptyListOfApprenticesIsInvalid()
        {
            _exampleValidCommand.Apprenticeships = new List<Apprenticeship>(0);

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
