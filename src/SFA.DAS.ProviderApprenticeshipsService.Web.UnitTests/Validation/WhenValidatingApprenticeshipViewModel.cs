﻿using System;

using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation
{
    [TestFixture]
    public class WhenValidatingApprenticeshipViewModel
    {
        private readonly ApprenticeshipViewModelValidator _validator = new ApprenticeshipViewModelValidator();
        private ApprenticeshipViewModel _validModel;

        [SetUp]
        public void Setup()
        {
            _validModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName"};
        }

        [Test]
        public void ULNMustBeNumericAnd10DigitsInLength()
        {
            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        [TestCase("abc123")]
        [TestCase("123456789")]
        [TestCase(" ")]
        public void ULNThatIsNotNumericOr10DigitsInLengthIsIvalid(string uln)
        {
            var viewModel = new ApprenticeshipViewModel { ULN = uln };

            var result = _validator.Validate(viewModel);

            result.IsValid.Should().BeFalse();
        }

        public void ULNThatStartsWithAZeroIsInvalid()
        {
            var viewModel = new ApprenticeshipViewModel { ULN = "0123456789" };

            var result = _validator.Validate(viewModel);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("123")]
        [TestCase("1")]
        public void CostIsWholeNumberGreaterThanZeroIsValid(string cost)
        {
            _validModel.Cost = cost;

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        [TestCase("123.12")]
        [TestCase("123.1")]
        [TestCase("123.0")]
        [TestCase("fdsfdfd")]
        [TestCase("123.000")]
        public void CostNotNumericOrIsNotAWholeNumber(string cost)
        {
            _validModel.Cost = cost;

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("0")]
        [TestCase("-0")]
        [TestCase("-123.12")]
        [TestCase("-123")]
        [TestCase("-123.1232")]
        [TestCase("-0.001")]
        public void CostThatIsZeroOrNegativeNumberIsInvalid(string cost)
        {
            _validModel.Cost = cost;

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
        }

        #region DateOfBirth

        [TestCase(31, 2, 13, "Enter a valid date of birth")]
        //[TestCase(31, 12, 1899, "Enter a valid date of birth")]
        [TestCase(5, null, 1998, "Enter a valid date of birth")]
        [TestCase(5, 9, null, "Enter a valid date of birth")]
        [TestCase(null, 9, 1998, "Enter a valid date of birth")]
        [TestCase(5, 9, -1, "Enter a valid date of birth")]
        [TestCase(0, 0, 0, "Enter a valid date of birth")]
        [TestCase(1, 18, 1998, "Enter a valid date of birth")]
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

        #endregion

        #region StartDate

        [TestCase(31, 2, 2121, "Enter a valid start date")]
        [TestCase(5, null, 2121, "Enter a valid start date")]
        [TestCase(5, 9, null, "Enter a valid start date")]
        [TestCase(5, 9, -1, "Enter a valid start date")]
        [TestCase(0, 0, 0, "Enter a valid start date")]
        [TestCase(1, 18, 2121, "Enter a valid start date")]
        //[TestCase(5, 9, 1998, "Learner start date must be in the future")]
        public void ShouldFailValidationForStartDateWith(int? day, int? month, int? year, string expected)
        {
            
            _validModel.StartDate = new DateTimeViewModel(day, month, year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(null, null, null)]
        [TestCase(5, 9, 2100)]
        [TestCase(1, 1, 2023)]
        [TestCase(null, 9, 2067)]
        public void ShouldNotFailValidationForStartDate(int? day, int? month, int? year)
        {
            _validModel.StartDate = new DateTimeViewModel(day, month, year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        #endregion

        #region PlannedEndDate

        [TestCase(31, 2, 2121, "Enter a valid training end date")]
        [TestCase(5, null, 2121, "Enter a valid training end date")]
        [TestCase(5, 9, null, "Enter a valid training end date")]
        [TestCase(5, 9, -1, "Enter a valid training end date")]
        [TestCase(0, 0, 0, "Enter a valid training end date")]
        [TestCase(1, 18, 2121, "Enter a valid training end date")]
        //[TestCase(5, 9, 1998, "Learner Planned end date must be in the future")]
        public void ShouldFailValidationForPlannedEndDateWith(int? day, int? month, int? year, string expected)
        {

            _validModel.EndDate = new DateTimeViewModel(day, month, year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(null, null, null)]
        [TestCase(5, 9, 2100)]
        [TestCase(1, 1, 2023)]
        [TestCase(null, 9, 2067)]
        public void ShouldNotFailValidationForPlannedEndDate(int? day, int? month, int? year)
        {
            _validModel.EndDate = new DateTimeViewModel(day, month, year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldFailIfStartDateIsAfterEndDate()
        {
            _validModel.StartDate = new DateTimeViewModel(DateTime.Parse("2121-05-10"));
            _validModel.EndDate = new DateTimeViewModel(DateTime.Parse("2120-05-10"));

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The end date must be later than the start date");
        }

        [Test]
        public void ShouldNotFailIfStartDateIsNull()
        {
            _validModel.StartDate = null;
            _validModel.EndDate = new DateTimeViewModel(DateTime.Parse("2120-05-10"));

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldNotFailIfEndDateIsNull()
        {
            _validModel.StartDate = new DateTimeViewModel(DateTime.Parse("2121-05-10"));
            _validModel.EndDate = null;

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        #endregion
    }
}
