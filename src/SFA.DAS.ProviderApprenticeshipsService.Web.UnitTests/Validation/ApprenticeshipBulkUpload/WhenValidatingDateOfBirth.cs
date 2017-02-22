using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    [TestFixture]
    public class WhenValidatingDateOfBirth : ApprenticeshipBulkUploadValidationTestBase
    {
        [Test]
        public void ShouldFailIfNullObjectReference()
        {
            ValidModel.DateOfBirth = null;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ShouldFailIfHasNoValuesSet()
        {
            ValidModel.DateOfBirth = new DateTimeViewModel(null, null, null); ;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [TestCase(31, 2, 13, "The <strong>Date of birth</strong> must be entered")]
        [TestCase(5, null, 1998, "The <strong>Date of birth</strong> must be entered")]
        [TestCase(5, 9, null, "The <strong>Date of birth</strong> must be entered")]
        [TestCase(null, 9, 1998, "The <strong>Date of birth</strong> must be entered")]
        [TestCase(5, 9, -1, "The <strong>Date of birth</strong> must be entered")]
        [TestCase(0, 0, 0, "The <strong>Date of birth</strong> must be entered")]
        [TestCase(1, 18, 1998, "The <strong>Date of birth</strong> must be entered")]
        public void ShouldFailValidationOnDateOfBirth(int? day, int? month, int? year, string expected)
        {
            ValidModel.DateOfBirth = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(5, 9, 1998)]
        [TestCase(1, 1, 1900)]
        public void ShouldNotFailValidationOnDateOfBirth(int? day, int? month, int? year)
        {
            ValidModel.DateOfBirth = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }
    }
}
