using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    [TestFixture]
    public class WhenValidatingTrainingDates : ApprenticeshipBulkUploadValidationTestBase
    {
        [Test]
        public void ShouldFailIfStartDateHasNullObjectReference()
        {
            ValidModel.StartDate = null;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ShouldFailIfEndDateHasNullObjectReference()
        {
            ValidModel.EndDate = null;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }


        [Test]
        public void ShouldIfStartDateBeforeMay2017()
        {
            ValidModel.StartDate = new DateTimeViewModel(30, 4, 2017);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The <strong>start date</strong> must not be earlier than 1 May 2017");
        }

        [TestCase(null, null, null, "The <strong>Learning start date</strong> must be entered")]
        [TestCase(31, 2, 2121, "The <strong>Learning start date</strong> must be entered")]
        [TestCase(5, null, 2121, "The <strong>Learning start date</strong> must be entered")]
        [TestCase(5, 9, null, "The <strong>Learning start date</strong> must be entered")]
        [TestCase(5, 9, -1, "The <strong>Learning start date</strong> must be entered")]
        [TestCase(0, 0, 0, "The <strong>Learning start date</strong> must be entered")]
        [TestCase(1, 18, 2121, "The <strong>Learning start date</strong> must be entered")]
        public void ShouldFailValidationForStartDate(int? day, int? month, int? year, string expected)
        {

            ValidModel.StartDate = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(5, 9, 2100)]
        [TestCase(1, 1, 2023)]
        [TestCase(null, 9, 2067)]
        public void ShouldNotFailValidationForStartDate(int? day, int? month, int? year)
        {
            ValidModel.StartDate = new DateTimeViewModel(day, month, year);
            ValidModel.EndDate = new DateTimeViewModel(1, 1, year + 1); // Make end date in the future

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        [TestCase(null, null, null, "The <strong>Learning planned end date</strong> must be entered and be in the format yyyy-mm-dd")]
        [TestCase(31, 2, 2121, "The <strong>Learning planned end date</strong> must be entered and be in the format yyyy-mm-dd")]
        [TestCase(5, null, 2121, "The <strong>Learning planned end date</strong> must be entered and be in the format yyyy-mm-dd")]
        [TestCase(5, 9, null, "The <strong>Learning planned end date</strong> must be entered and be in the format yyyy-mm-dd")]
        [TestCase(5, 9, -1, "The <strong>Learning planned end date</strong> must be entered and be in the format yyyy-mm-dd")]
        [TestCase(0, 0, 0, "The <strong>Learning planned end date</strong> must be entered and be in the format yyyy-mm-dd")]
        [TestCase(1, 18, 2121, "The <strong>Learning planned end date</strong> must be entered and be in the format yyyy-mm-dd")]
        [TestCase(5, 9, 1998, "The <strong>Learning planned end date</strong> must not be on or before the Learning start date")]
        public void ShouldFailValidationForPlannedEndDate(int? day, int? month, int? year, string expected)
        {

            ValidModel.EndDate = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(5, 9, 2100)]
        [TestCase(1, 1, 2023)]
        [TestCase(null, 9, 2067)]
        public void ShouldNotFailValidationForPlannedEndDate(int? day, int? month, int? year)
        {
            ValidModel.EndDate = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        // TODO: LWA - Implement when have control over current date
        //[Test]
        //public void ShouldFailValidationForPlanedEndDateWithCurrentDate()
        //{
        //    var date = DateTime.Now;
        //    ValidModel.StartDate = new DateTimeViewModel(DateTime.Now.AddDays(-30));
        //    ValidModel.EndDate = new DateTimeViewModel(date.Day, date.Month, date.Year);

        //    var result = Validator.Validate(ValidModel);

        //    result.IsValid.Should().BeFalse();
        //    result.Errors[0].ErrorMessage.Should().Be("The <strong>Learning planned end date</strong> must be entered and be in the format yyyy-mm-dd");
        //}

        [Test]

        public void ShouldFailIfStartDateIsAfterEndDate()
        {
            ValidModel.StartDate = new DateTimeViewModel(DateTime.Parse("2121-05-10"));
            ValidModel.EndDate = new DateTimeViewModel(DateTime.Parse("2120-05-10"));

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The <strong>Learning planned end date</strong> must not be on or before the Learning start date");
        }

        [Test]
        public void ShouldFailIfStartDateIsNull()
        {
            ValidModel.StartDate = null;
            ValidModel.EndDate = new DateTimeViewModel(DateTime.Parse("2120-05-10"));

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ShouldFailIfEndDateIsNull()
        {
            ValidModel.StartDate = new DateTimeViewModel(DateTime.Parse("2121-05-10"));
            ValidModel.EndDate = null;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }
    }
}
