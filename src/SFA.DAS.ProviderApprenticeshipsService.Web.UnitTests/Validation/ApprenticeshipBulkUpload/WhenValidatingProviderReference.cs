using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    [TestFixture]
    public class WhenValidatingProviderReference : ApprenticeshipBulkUploadValidationTestBase
    {
        [TestCase("")]
        [TestCase(null)]
        public void ProviderReferenceIsOptional(string reference)
        {
            ValidModel.ProviderRef = reference;

            var result = Validator.Validate(ValidModel);

            result.Errors.Count.Should().Be(0);
        }

        [Test]
        public void ProviderReferenceCanBeAMaximumOf20Characters()
        {
            ValidModel.ProviderRef = new string('*', 21);

            var result = Validator.Validate(ValidModel);

            result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.Should().Be("You must enter a valid <strong>Provider reference code</strong> - you can add up to 20 characters");
            result.Errors[0].ErrorCode.Should().Be("ProviderRef_01");
        }

        [Test]
        public void ProviderReferenceLessCanBeLessThan20Characters()
        {
            ValidModel.ProviderRef = "A valid reference";

            var result = Validator.Validate(ValidModel);

            result.Errors.Count.Should().Be(0);
        }
    }
}
