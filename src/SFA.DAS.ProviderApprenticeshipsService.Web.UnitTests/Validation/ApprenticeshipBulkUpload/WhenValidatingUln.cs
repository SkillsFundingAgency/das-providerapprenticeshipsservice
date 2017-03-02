using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    [TestFixture]
    public class WhenValidatingUln : ApprenticeshipBulkUploadValidationTestBase
    {
        [TestCase("")]
        [TestCase(null)]
        public void ULNMustNotBeEmpty(string uln)
        {
            ValidModel.ULN = uln;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("abc123")]
        [TestCase("123456789")]
        [TestCase(" ")]
        [TestCase("9999999999")]
        public void ULNThatIsNotNumericOr10DigitsInLengthIsInvalid(string uln)
        {
            ValidModel.ULN = uln;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }
        
        [Test]
        public void ULN9999999999IsNotValid()
        {
            ValidModel.ULN = "9999999999";

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ULNThatStartsWithAZeroIsInvalid()
        {
            ValidModel.ULN = "0123456789";

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ULNWithValidValueIsValid()
        {
            ValidModel.ULN = "1234567898";

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }
    }
}
