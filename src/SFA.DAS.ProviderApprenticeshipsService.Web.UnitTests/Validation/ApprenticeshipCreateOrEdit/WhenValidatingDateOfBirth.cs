using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingDateOfBirth : ApprenticeshipValidationTestBase
    {
        [TestCase(31, 2, 13, "The Date of birth must be entered")]
        [TestCase(5, null, 1998, "The Date of birth must be entered")]
        [TestCase(5, 9, null, "The Date of birth must be entered")]
        [TestCase(null, 9, 1998, "The Date of birth must be entered")]
        [TestCase(5, 9, -1, "The Date of birth must be entered")]
        [TestCase(0, 0, 0, "The Date of birth must be entered")]
        [TestCase(1, 18, 1998, "The Date of birth must be entered")]
        public void ShouldFailValidationOnDateOfBirth(int? day, int? month, int? year, string expected)
        {
            ValidModel.DateOfBirth = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(null, null, null)]
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
