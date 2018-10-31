using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    [TestFixture]
    public class WhenValidatingFirstName : ApprenticeshipBulkUploadValidationTestBase
    {

        [Test]
        public void TestFirstNamesNotNull()
        {
            ValidModel.ApprenticeshipViewModel.FirstName = null;

            var result = Validator.Validate(ValidModel);
            result.Errors.Count.Should().Be(1);

            result.Errors[0].ErrorMessage.Should().Be("<strong>First name</strong> must be entered");
        }

        [Test]
        public void FirstNameShouldNotBeEmpty()
        {
            ValidModel.ApprenticeshipViewModel.FirstName = " ";

            var result = Validator.Validate(ValidModel);
            result.Errors.Count.Should().Be(1);

            result.Errors[0].ErrorMessage.Should().Be("<strong>First name</strong> must be entered");
        }

        [TestCase(99, 0)]
        [TestCase(100, 0)]
        [TestCase(101, 1)]
        public void TestLengthOfFirstName(int length, int expectedErrorCount)
        {
            ValidModel.ApprenticeshipViewModel.FirstName = new string('*', length);

            var result = Validator.Validate(ValidModel);

            result.Errors.Count.Should().Be(expectedErrorCount);

            if (expectedErrorCount > 0)
            {
                result.Errors[0].ErrorMessage.Should().Be("You must enter a <strong>first name</strong> that's no longer than 100 characters");
            }
        }
    }
}