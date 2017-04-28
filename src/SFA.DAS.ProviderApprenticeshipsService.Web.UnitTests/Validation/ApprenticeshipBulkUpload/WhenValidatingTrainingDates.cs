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
            ValidModel.ApprenticeshipViewModel.StartDate = null;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ShouldFailIfEndDateHasNullObjectReference()
        {
            ValidModel.ApprenticeshipViewModel.EndDate = null;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }


        [Test]
        public void ShouldIfStartDateBeforeMay2017()
        {
            ValidModel.ApprenticeshipViewModel.StartDate = new DateTimeViewModel(30, 4, 2017);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The <strong>start date</strong> must not be earlier than May 2017");
        }

        [TestCase(null, null, null)]
        [TestCase(31, 2, 2121)]
        [TestCase(5, null, 2121)]
        [TestCase(5, 9, null)]
        [TestCase(5, 9, -1)]
        [TestCase(0, 0, 0)]
        [TestCase(1, 18, 2121)]
        public void ShouldFailValidationForStartDate(int? day, int? month, int? year)
        {
            var expected = "You must enter the <strong>start date</strong>, for example 2017-09";
            ValidModel.ApprenticeshipViewModel.StartDate = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(5, 9, 2100)]
        [TestCase(1, 1, 2023)]
        [TestCase(null, 9, 2067)]
        public void ShouldNotFailValidationForStartDate(int? day, int? month, int? year)
        {
            ValidModel.ApprenticeshipViewModel.StartDate = new DateTimeViewModel(day, month, year);
            ValidModel.ApprenticeshipViewModel.EndDate = new DateTimeViewModel(1, 1, year + 1); // Make end date in the future

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        [TestCase(null, null, null, "You must enter the <strong>end date</strong>, for example 2019-02")]
        [TestCase(31, 2, 2121, "You must enter the <strong>end date</strong>, for example 2019-02")]
        [TestCase(5, null, 2121, "You must enter the <strong>end date</strong>, for example 2019-02")]
        [TestCase(5, 9, null, "You must enter the <strong>end date</strong>, for example 2019-02")]
        [TestCase(5, 9, -1, "You must enter the <strong>end date</strong>, for example 2019-02")]
        [TestCase(0, 0, 0, "You must enter the <strong>end date</strong>, for example 2019-02")]
        [TestCase(1, 18, 2121, "You must enter the <strong>end date</strong>, for example 2019-02")]
        [TestCase(5, 9, 1998, "You must not enter an <strong>end date</strong> that's earlier than the start date")]
        public void ShouldFailValidationForPlannedEndDate(int? day, int? month, int? year, string expected)
        {

            ValidModel.ApprenticeshipViewModel.EndDate = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(5, 9, 2100)]
        [TestCase(1, 1, 2023)]
        [TestCase(null, 9, 2067)]
        public void ShouldNotFailValidationForPlannedEndDate(int? day, int? month, int? year)
        {
            ValidModel.ApprenticeshipViewModel.EndDate = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldFailValidationForPlanedEndDateWithCurrentDate()
        {
            MockCurrentDateTime.SetupGet(x => x.Now).Returns(new DateTime(2017, 7, 15));
            ValidModel.ApprenticeshipViewModel.StartDate = new DateTimeViewModel(new DateTime(2017, 7, 20));
            ValidModel.ApprenticeshipViewModel.EndDate = new DateTimeViewModel(new DateTime(2017, 7, 18));

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("You must not enter an <strong>end date</strong> that's earlier than the start date");
        }

        [Test]

        public void ShouldFailIfStartDateIsAfterEndDate()
        {
            ValidModel.ApprenticeshipViewModel.StartDate = new DateTimeViewModel(DateTime.Parse("2121-05-10"));
            ValidModel.ApprenticeshipViewModel.EndDate = new DateTimeViewModel(DateTime.Parse("2120-05-10"));

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("You must not enter an <strong>end date</strong> that's earlier than the start date");
        }

        [Test]
        public void ShouldFailIfStartDateIsNull()
        {
            ValidModel.ApprenticeshipViewModel.StartDate = null;
            ValidModel.ApprenticeshipViewModel.EndDate = new DateTimeViewModel(DateTime.Parse("2120-05-10"));

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ShouldFailIfEndDateIsNull()
        {
            ValidModel.ApprenticeshipViewModel.StartDate = new DateTimeViewModel(DateTime.Parse("2121-05-10"));
            ValidModel.ApprenticeshipViewModel.EndDate = null;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }
    }
}
