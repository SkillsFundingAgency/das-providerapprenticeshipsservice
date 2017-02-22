using System;
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

        [Test]
        public void ShouldFailIfNotAtLeast15AtStartOfTraining()
        {
            ValidModel.DateOfBirth = new DateTimeViewModel(new DateTime(2004, 06, 03));
            ValidModel.StartDate = new DateTimeViewModel(null, 6, 2019);
            ValidModel.EndDate = new DateTimeViewModel(null, 6, 2020);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The apprentice must be at least 15 years old at the start of the programme");
        }
    }
}
