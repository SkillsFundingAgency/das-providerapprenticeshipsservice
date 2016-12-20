using System.Net.NetworkInformation;
﻿using System;

using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests
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
            var viewModel = new ApprenticeshipViewModel { ULN = uln, Cost = string.Empty};

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

        [Test]
        public void TestNamesNotNull()
        {
            _validModel.FirstName = null;
            _validModel.LastName = null;

            var result = _validator.Validate(_validModel);
            result.Errors.Count.Should().Be(2);
            
            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("Enter a first name");
            result.Errors[0].ErrorCode.ShouldAllBeEquivalentTo("GivenNames_01");
            result.Errors[1].ErrorMessage.ShouldBeEquivalentTo("Enter a last name");
            result.Errors[1].ErrorCode.ShouldAllBeEquivalentTo("FamilyName_01");
            
        }

        [TestCase(99, 0)]
        [TestCase(100, 0)]
        [TestCase(101, 2)]
        public void TestLengthOfNames(int length, int expectedErrorCount)
        {
            _validModel.FirstName = new string('*', length);
            _validModel.LastName = new string('*', length);

            var result = _validator.Validate(_validModel);
            result.Errors.Count.Should().Be(expectedErrorCount);
            if (expectedErrorCount > 0)
            {
                result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("First name cannot contain more then 100 chatacters");
                result.Errors[0].ErrorCode.ShouldAllBeEquivalentTo("GivenNames_02");
                result.Errors[1].ErrorMessage.ShouldBeEquivalentTo("Last name cannot contain more then 100 chatacters");
                result.Errors[1].ErrorCode.ShouldAllBeEquivalentTo("FamilyName_02");
            }
        }

        [Test]
        public void NationalInsurnceNumberCanBeNull()
        {
            _validModel.NINumber = null;

            _validator
                .Validate(_validModel)
                .Errors.Count.Should().Be(0);
        }

        [TestCase("DE123456A", Description = "First char not allowed")]
        [TestCase("FE123456A", Description = "First char not allowed")]
        [TestCase("SO123456A", Description = "Second char not allowed")]
        [TestCase("SQ123456A", Description = "Second char not allowed")]

        [TestCase("SE12345A", Description = "Not enough numbers")]
        [TestCase("SE123456E", Description = "Last char not allowed")]
        [TestCase("SE123456", Description = "Not enought chars")]
        public void NationalInsurnceNumberShouldFail(string nino)
        {
            _validModel.NINumber = nino;

            var result = _validator
                .Validate(_validModel);

            result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.ShouldAllBeEquivalentTo("Enter a valid national insurance number");
        }

        [TestCase("SE1234567A", Description = "Too many numbers")]
        [TestCase("SE123456  ", Description = "Too many chars")]
        public void NationalInsurnceNumberShouldFailTooManyChars(string nino)
        {
            _validModel.NINumber = nino;

            var result = _validator
                .Validate(_validModel);

            result.Errors.Count.Should().Be(2);
            result.Errors[0].ErrorMessage.ShouldAllBeEquivalentTo("National insurance number needs to be 10 characters long");
        }


        [TestCase("SE123456 ")]
        [TestCase("SE123456A")]
        public void NationalInsurnceNumberShouldValidate(string nino)
        {
            _validModel.NINumber = nino;

            var result = _validator
                .Validate(_validModel);
            result.Errors.Count.Should().Be(0);
        }

        [Test]
        public void EmployerRef()
        {
            _validModel.ProviderRef = string.Empty;

            var result = _validator
                .Validate(_validModel);
            result.Errors.Count.Should().Be(0);
        }

        [Test]
        public void EmployerRef2()
        {
            _validModel.ProviderRef = null;

            var result = _validator
                .Validate(_validModel);
            result.Errors.Count.Should().Be(0);
        }

        [Test]
        public void EmployerRef3()
        {
            _validModel.ProviderRef = "abba 123 !";

            var result = _validator
                .Validate(_validModel);
            result.Errors.Count.Should().Be(0);
        }

        [Test]
        public void EmployerRef4()
        {
            _validModel.ProviderRef = new string('*', 21);

            var result = _validator
                .Validate(_validModel);
            result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.Should().Be("Provider reference must not contain more than 20 characters");
            result.Errors[0].ErrorCode.Should().Be("ProvRef_01");
        }

        #region DateOfBirth

        [TestCase(31, 2, 13, "Date of birth is not valid")]
        //[TestCase(31, 12, 1899, "Date of birth is not valid")]
        [TestCase(5, null, 1998, "Date of birth is not valid")]
        [TestCase(5, 9, null, "Date of birth is not valid")]
        [TestCase(null, 9, 1998, "Date of birth is not valid")]
        [TestCase(5, 9, -1, "Date of birth is not valid")]
        [TestCase(0, 0,  0, "Date of birth is not valid")]
        [TestCase(1, 18, 1998, "Date of birth is not valid")]
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
        public void ShouldFailValidationOnDateOfBirth()
        {
            var date = DateTime.Now;
            _validModel.DateOfBirth = new DateTimeViewModel(date.Day, date.Month, date.Year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("Date of birth must be in the past");
        }

        #endregion

        #region StartDate

        [TestCase(31, 2, 2121, "Start date is not a valid date")]
        [TestCase(5, null, 2121, "Start date is not a valid date")]
        [TestCase(5, 9, null, "Start date is not a valid date")]
        [TestCase(5, 9, -1, "Start date is not a valid date")]
        [TestCase(0, 0, 0, "Start date is not a valid date")]
        [TestCase(1, 18, 2121, "Start date is not a valid date")]
        [TestCase(5, 9, 1998, "Learner start date must be in the future")]
        public void ShouldFailValidationForStartDate(int? day, int? month, int? year, string expected)
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


        [Test]
        public void ShouldFailValidationForStartDate()
        {
            var date = DateTime.Now;
            _validModel.StartDate = new DateTimeViewModel(date.Day, date.Month, date.Year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("Learner start date must be in the future");
        }

        #endregion

        #region PlanedEndDate

        [TestCase(31, 2, 2121, "Planed end date is not a valid date")]
        [TestCase(5, null, 2121, "Planed end date is not a valid date")]
        [TestCase(5, 9, null, "Planed end date is not a valid date")]
        [TestCase(5, 9, -1, "Planed end date is not a valid date")]
        [TestCase(0, 0, 0, "Planed end date is not a valid date")]
        [TestCase(1, 18, 2121, "Planed end date is not a valid date")]
        [TestCase(5, 9, 1998, "Learner planed end date must be in the future")]
        public void ShouldFailValidationForPlanedEndDate(int? day, int? month, int? year, string expected)
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
        public void ShouldNotFailValidationForPlanedEndDate(int? day, int? month, int? year)
        {
            _validModel.EndDate = new DateTimeViewModel(day, month, year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldFailValidationForPlanedEndDate()
        {
            var date = DateTime.Now;
            _validModel.EndDate = new DateTimeViewModel(date.Day, date.Month, date.Year);

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("Learner planed end date must be in the future");
        }

        [Test]
        public void ShouldFailIfStartDateIsAfterEndDate()
        {
            _validModel.StartDate = new DateTimeViewModel(DateTime.Parse("2121-05-10"));
            _validModel.EndDate = new DateTimeViewModel(DateTime.Parse("2120-05-10"));

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("Learner planed end date must be greater than start date");
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
