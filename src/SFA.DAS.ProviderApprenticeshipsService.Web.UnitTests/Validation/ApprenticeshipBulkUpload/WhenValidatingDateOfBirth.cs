using System;
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
            ValidModel.ApprenticeshipViewModel.DateOfBirth = null;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ShouldFailIfHasNoValuesSet()
        {
            ValidModel.ApprenticeshipViewModel.DateOfBirth = new DateTimeViewModel(null, null, null); ;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [TestCase(31, 2, 13)]
        [TestCase(5, null, 1998)]
        [TestCase(5, 9, null)]
        [TestCase(null, 9, 1998)]
        [TestCase(5, 9, -1)]
        [TestCase(0, 0, 0)]
        [TestCase(1, 18, 1998)]
        public void ShouldFailValidationOnDateOfBirth(int? day, int? month, int? year)
        {
            var expected = "You must enter the apprentice's <strong>date of birth</strong> using the format yyyy-mm-dd, for example 2001-04-23";
            ValidModel.ApprenticeshipViewModel.DateOfBirth = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(5, 9, 1998)]
        public void ShouldNotFailValidationOnDateOfBirth(int? day, int? month, int? year)
        {
            ValidModel.ApprenticeshipViewModel.DateOfBirth = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldFailIfNotAtLeast15AtStartOfTraining()
        {
            ValidModel.ApprenticeshipViewModel.DateOfBirth = new DateTimeViewModel(new DateTime(2004, 06, 03));
            ValidModel.ApprenticeshipViewModel.StartDate = new DateTimeViewModel(null, 6, 2019);
            ValidModel.ApprenticeshipViewModel.EndDate = new DateTimeViewModel(null, 6, 2020);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The apprentice's <strong>date of birth</strong> must show that they're at least 15 years old at the start of their training");
        }

        [Test]
        public void ShouldFailIfAgeIs115AtStartOfTraining()
        {
            ValidModel.ApprenticeshipViewModel.DateOfBirth = new DateTimeViewModel(new DateTime(1904, 06, 01));
            ValidModel.ApprenticeshipViewModel.StartDate = new DateTimeViewModel(null, 6, 2019);
            ValidModel.ApprenticeshipViewModel.EndDate = new DateTimeViewModel(null, 6, 2020);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("Enter a valid year - the apprentice must be younger than 115 at the start of the current teaching year");
        }

        [Test]
        public void ShouldFailIfAgeIsMoreThenTheMinimumAllowedAge()
        {
            ValidModel.ApprenticeshipViewModel.DateOfBirth = new DateTimeViewModel(new DateTime(1899, 12, 31));
            ValidModel.ApprenticeshipViewModel.StartDate = new DateTimeViewModel(null, 6, 2017);
            ValidModel.ApprenticeshipViewModel.EndDate = new DateTimeViewModel(null, 6, 2020);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The Date of birth is not valid");
        }
    }
}
