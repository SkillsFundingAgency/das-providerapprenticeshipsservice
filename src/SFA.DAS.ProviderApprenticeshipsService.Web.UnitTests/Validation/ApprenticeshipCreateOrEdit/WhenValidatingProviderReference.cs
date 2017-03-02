using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingProviderReference : ApprenticeshipValidationTestBase
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
            result.Errors[0].ErrorMessage.Should().Be("The Reference must be 20 characters or fewer");
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
