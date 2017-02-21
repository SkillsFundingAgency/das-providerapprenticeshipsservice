using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingDateOfBirth
    {
        private readonly ApprenticeshipViewModelValidator _validator = new ApprenticeshipViewModelValidator(new WebApprenticeshipValidationText());
        private ApprenticeshipViewModel _validModel;

        [SetUp]
        public void Setup()
        {
            _validModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
        }

        [TestCase(31, 2, 13, "The Date of birth must be entered")]
        [TestCase(5, null, 1998, "The Date of birth must be entered")]
        [TestCase(5, 9, null, "The Date of birth must be entered")]
        [TestCase(null, 9, 1998, "The Date of birth must be entered")]
        [TestCase(5, 9, -1, "The Date of birth must be entered")]
        [TestCase(0, 0, 0, "The Date of birth must be entered")]
        [TestCase(1, 18, 1998, "The Date of birth must be entered")]
        public void ShouldFailValidationOnDateOfBirth(int? day, int? month, int? year, string expected)
        {
            _validModel.DateOfBirth = new DateTimeViewModel(day, month, year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(null, null, null)]
        [TestCase(5, 9, 1998)]
        [TestCase(1, 1, 1900)]
        public void ShouldNotFailValidationOnDateOfBirth(int? day, int? month, int? year)
        {
            _validModel.DateOfBirth = new DateTimeViewModel(day, month, year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldFailValidationOnDateOfBirthWithTodayAsValue()
        {
            var date = DateTime.Now;
            _validModel.DateOfBirth = new DateTimeViewModel(date.Day, date.Month, date.Year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The date of birth must be in the past");
        }
    }
}
