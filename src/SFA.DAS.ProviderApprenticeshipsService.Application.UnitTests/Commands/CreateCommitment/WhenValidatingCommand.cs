using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.CreateCommitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests.Commands.CreateCommitment
{
    public class WhenValidatingCommand
    {
        private CreateCommitmentCommandValidator _validator;
        private CreateCommitmentCommand _validCommand;

        [SetUp]
        public void Arrange()
        {
            _validator = new CreateCommitmentCommandValidator();

            _validCommand = new CreateCommitmentCommand
            {
                Commitment = new Commitment
                {
                    LegalEntityId = "001",
                    LegalEntityName = "Test Legal Entity Name",
                    EmployerAccountId = 2,
                    ProviderId = 3,
                    ProviderName = "Test provider"
                },
                UserId = "TestUser"
            };
        }

        [Test]
        public void ThenValidIfAllPropertiesSpecified()
        {
            _validCommand.Commitment = null;

            var result = _validator.Validate(TestHelper.Clone(_validCommand));

            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void ThenIfCommitmentNotSpecifiedIsInvalid()
        {
            _validCommand.Commitment = null;

            var result = _validator.Validate(TestHelper.Clone(_validCommand));

            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void ThenIfLegalEntityIdNotSpecifiedIsInvalid()
        {
            _validCommand.Commitment.LegalEntityId = "";

            var result = _validator.Validate(TestHelper.Clone(_validCommand));

            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void ThenIfLegalEntityNameNotSpecifiedIsInvalid()
        {
            _validCommand.Commitment.LegalEntityName = "";

            var result = _validator.Validate(TestHelper.Clone(_validCommand));

            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void ThenIfEmployerAccountIdNotSpecifiedIsInvalid()
        {
            _validCommand.Commitment.EmployerAccountId = 0;

            var result = _validator.Validate(TestHelper.Clone(_validCommand));

            Assert.IsFalse(result.IsValid);
        }

        [TestCase(null)]
        [TestCase(0)]
        public void ThenIfProviderIdNotSpecifiedIsInvalid(long? providerId)
        {
            _validCommand.Commitment.ProviderId = providerId;

            var result = _validator.Validate(TestHelper.Clone(_validCommand));

            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void ThenIfProviderNameNotSpecifiedIsInvalid()
        {
            _validCommand.Commitment.ProviderName = "";

            var result = _validator.Validate(TestHelper.Clone(_validCommand));

            Assert.IsFalse(result.IsValid);
        }
    }
}
