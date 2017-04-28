using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    [TestFixture]
    public class WhenValidatingLastName : ApprenticeshipBulkUploadValidationTestBase
    { 
        [Test]
        public void TestLastNameNotNull()
        {
            ValidModel.ApprenticeshipViewModel.LastName = null;

            var result = Validator.Validate(ValidModel);

            result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("<strong>Last name</strong> must be entered");
        }

        [Test]
        public void LastNameShouldNotBeEmpty()
        {
            ValidModel.ApprenticeshipViewModel.LastName = " ";

            var result = Validator.Validate(ValidModel);
            result.Errors.Count.Should().Be(1);

            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("<strong>Last name</strong> must be entered");
        }

        [TestCase(99, 0)]
        [TestCase(100, 0)]
        [TestCase(101, 1)]
        public void TestLengthOfLastName(int length, int expectedErrorCount)
        {
            ValidModel.ApprenticeshipViewModel.LastName = new string('*', length);

            var result = Validator.Validate(ValidModel);

            result.Errors.Count.Should().Be(expectedErrorCount);

            if (expectedErrorCount > 0)
            {
                result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("You must enter a <strong>last name</strong> that's no longer than 100 characters");
            }
        }
    }
}
