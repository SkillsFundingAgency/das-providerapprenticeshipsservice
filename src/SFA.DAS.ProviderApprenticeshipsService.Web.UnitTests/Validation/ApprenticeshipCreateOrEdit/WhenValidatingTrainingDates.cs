﻿using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingTrainingDates : ApprenticeshipValidationTestBase
    {
        [Test]
        public void ShouldIfStartDateBeforeMay2017()
        {
            ValidModel.StartDate = new DateTimeViewModel(30, 4, 2017);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The start date must not be earlier than May 2017");
        }

        [TestCase(31, 2, 2121, "The start date is not valid")]
        [TestCase(5, null, 2121, "The start date is not valid")]
        [TestCase(5, 9, null, "The start date is not valid")]
        [TestCase(5, 9, -1, "The start date is not valid")]
        [TestCase(0, 0, 0, "The start date is not valid")]
        [TestCase(1, 18, 2121, "The start date is not valid")]
        //[TestCase(5, 9, 1998, "Learner start date must be in the future")]
        public void ShouldFailValidationForStartDate(int? day, int? month, int? year, string expected)
        {

            ValidModel.StartDate = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(null, null, null)]
        [TestCase(5, 9, 2018)]
        [TestCase(1, 1, 2018)]
        [TestCase(null, 9, 2018)]
        public void ShouldNotFailValidationForStartDate(int? day, int? month, int? year)
        {
            ValidModel.StartDate = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        [TestCase(31, 2, 2121, "The end date is not valid")]
        [TestCase(5, null, 2121, "The end date is not valid")]
        [TestCase(5, 9, null, "The end date is not valid")]
        [TestCase(5, 9, -1, "The end date is not valid")]
        [TestCase(0, 0, 0, "The end date is not valid")]
        [TestCase(1, 18, 2121, "The end date is not valid")]
        public void ShouldFailValidationForPlannedEndDate(int? day, int? month, int? year, string expected)
        {
            ValidModel.EndDate = new DateTimeViewModel(day, month, year);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [TestCase(0, 1)]
        [TestCase(0, 28)]
        [TestCase(-1, 1)]
        [TestCase(-1, 28)]
        [TestCase(-12, 1)]
        public void ShouldFailValidationForPlanedEndDateInPast(int monthsToAdd, int currentDay)
        {
            CurrentDateTime.Setup(x => x.Now).Returns(new DateTime(2019, 3, currentDay));
            var endDate = new DateTimeViewModel(CurrentDateTime.Object.Now.AddMonths(monthsToAdd)) { Day = 1 };

            var result = Validator.CheckEndDateInFuture(endDate);

            result.HasValue.Should().BeTrue();
            result.Value.Key.Should().Be("EndDate");
            result.Value.Value.Should().Be("The end date must not be in the past");
        }

        [TestCase(1, 1)]
        [TestCase(12, 1)]
        public void ShouldPassValidationForPlanedEndDateInFuture(int monthsToAdd, int currentDay)
        {
            CurrentDateTime.Setup(x => x.Now).Returns(new DateTime(2019, 3, currentDay));
            var endDate = new DateTimeViewModel(CurrentDateTime.Object.Now.AddMonths(monthsToAdd)) { Day = 1 };

            var result = Validator.CheckEndDateInFuture(endDate);

            result.HasValue.Should().BeFalse();
        }

        [TestCase(1, 6, 2018, 1, 5, 2018)]
        [TestCase(1, 6, 2018, 1, 6, 2018)]
        public void ShouldFailValidationWhenEndDateIsOnOrBeforeStartDate(
            int? startDay, int? startMonth, int? startYear,
            int? endDay, int? endMonth, int? endYear)
        {
            const string expected = "The end date must not be on or before the start date";
            ValidModel.StartDate = new DateTimeViewModel(startDay, startMonth, startYear);
            ValidModel.EndDate = new DateTimeViewModel(endDay, endMonth, endYear);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expected);
        }

        [Test]
        public void ShouldNotFailIfStartDateIsNull()
        {
            ValidModel.StartDate = null;
            ValidModel.EndDate = new DateTimeViewModel(DateTime.Parse("2120-05-10"));

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldNotFailIfEndDateIsNull()
        {
            ValidModel.StartDate = new DateTimeViewModel(CurrentDateTime.Object.Now);
            ValidModel.EndDate = null;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldFailValidationIfTransferStartDateIsBeforeMay2018()
        {
            ValidModel.IsPaidForByTransfer = true;
            ValidModel.StartDate = new DateTimeViewModel(1, 4, 2018);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("Apprentices funded through a transfer can't start earlier than May 2018");
        }
    }
}
