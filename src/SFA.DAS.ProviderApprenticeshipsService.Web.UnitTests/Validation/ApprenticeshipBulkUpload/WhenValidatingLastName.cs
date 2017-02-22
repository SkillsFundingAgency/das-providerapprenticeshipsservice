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
            ValidModel.LastName = null;

            var result = Validator.Validate(ValidModel);

            result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("You must enter the apprentice's <strong>last name</strong> - you can add up to 100 characters");
        }

        [Test]
        public void LastNameShouldNotBeEmpty()
        {
            ValidModel.LastName = " ";

            var result = Validator.Validate(ValidModel);
            result.Errors.Count.Should().Be(1);

            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("You must enter the apprentice's <strong>last name</strong> - you can add up to 100 characters");
        }

        [TestCase(99, 0)]
        [TestCase(100, 0)]
        [TestCase(101, 1)]
        public void TestLengthOfLastName(int length, int expectedErrorCount)
        {
            ValidModel.LastName = new string('*', length);

            var result = Validator.Validate(ValidModel);

            result.Errors.Count.Should().Be(expectedErrorCount);

            if (expectedErrorCount > 0)
            {
                result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("You must enter the apprentice's <strong>last name</strong> - you can add up to 100 characters");
            }
        }
    }
}
