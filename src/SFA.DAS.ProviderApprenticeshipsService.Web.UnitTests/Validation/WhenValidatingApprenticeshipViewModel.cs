using System;

using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation
{
    [TestFixture]
    public class WhenValidatingApprenticeshipViewModel
    {
        private readonly ApprenticeshipViewModelValidator _validator = new ApprenticeshipViewModelValidator(new WebApprenticeshipValidationText());
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
        [TestCase("9999999999")]
        public void ULNThatIsNotNumericOr10DigitsInLengthIsInvalid(string uln)
        {
            var viewModel = new ApprenticeshipViewModel { ULN = uln, Cost = string.Empty};

            var result = _validator.Validate(viewModel);

            result.IsValid.Should().BeFalse();
        }

        
        [Test]
        public void ULN9999999999IsNotValid()
        {
            var viewModel = new ApprenticeshipViewModel { ULN = "9999999999", Cost = string.Empty };

            var result = _validator.Validate(viewModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ULNThatStartsWithAZeroIsInvalid()
        {
            var viewModel = new ApprenticeshipViewModel { ULN = "0123456789" };

            var result = _validator.Validate(viewModel);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("1000")]
        [TestCase("1234")]
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

        [TestCase("1234567")]
        public void CostMustContain6DigitsOrLess(string value)
        {
            _validModel.Cost = value;

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
        }

        [TestCase("123,567")]
        public void CostMustContain6DigitsOrLessIgnoringCommas(string value)
        {
            _validModel.Cost = value;

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void TestNamesNotNull()
        {
            _validModel.FirstName = null;
            _validModel.LastName = null;

            var result = _validator.Validate(_validModel);
            result.Errors.Count.Should().Be(2);
            
            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("First name must be entered");
            result.Errors[1].ErrorMessage.ShouldBeEquivalentTo("Last name must be entered");
        }

        [Test]
        public void NamesShouldNotBeEmpty()
        {
            _validModel.FirstName = " ";
            _validModel.LastName = " ";

            var result = _validator.Validate(_validModel);
            result.Errors.Count.Should().Be(2);

            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("First name must be entered");
            result.Errors[1].ErrorMessage.ShouldBeEquivalentTo("Last name must be entered");
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
                result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("You must enter a first name that's no longer than 100 characters");
                result.Errors[1].ErrorMessage.ShouldBeEquivalentTo("You must enter a last name that's no longer than 100 characters");
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

        //[TestCase("DE123456A", Description = "First char not allowed")]
        //[TestCase("FE123456A", Description = "First char not allowed")]
        //[TestCase("SO123456A", Description = "Second char not allowed")]
        //[TestCase("SQ123456A", Description = "Second char not allowed")]

        //[TestCase("SE12345A", Description = "Not enough numbers")]
        //[TestCase("SE123456E", Description = "Last char not allowed")]
        //[TestCase("SE123456", Description = "Not enought chars")]
        //public void NationalInsurnceNumberShouldFail(string nino)
        //{
        //    _validModel.NINumber = nino;

        //    var result = _validator
        //        .Validate(_validModel);

        //    result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.Should().Be("Enter a valid National insurance number");
        //}

        //[TestCase("SE1234567A", Description = "Too many numbers")]
        //[TestCase("SE123456  ", Description = "Too many chars")]
        //public void NationalInsurnceNumberShouldFailTooManyChars(string nino)
        //{
        //    _validModel.NINumber = nino;

        //    var result = _validator
        //        .Validate(_validModel);

        //    result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.Should().Be("The National Insurance number must be entered and must not be more than 9 characters in length");
        //}

        //[TestCase("SE123456 ")]
        //[TestCase("SE123456A")]
        //public void NationalInsurnceNumberShouldValidate(string nino)
        //{
        //    _validModel.NINumber = nino;

        //    var result = _validator
        //        .Validate(_validModel);
        //    result.Errors.Count.Should().Be(0);
        //}

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
            result.Errors[0].ErrorMessage.Should().Be("The Reference must be 20 characters or fewer");
            result.Errors[0].ErrorCode.Should().Be("ProviderRef_01");
        }
          
        public void CostContainingValidCommaSeparatorIsValid()
        {
            _validModel.Cost = "1,234";

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        [TestCase(",111")]
        [TestCase("1,22")]
        [TestCase("1,22,222")]
        public void CostThatContainsBadlyFormatedCommaSeparatorsIsInvalid(string cost)
        {
            _validModel.Cost = cost;

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
        }
          
        #region DateOfBirth

        [TestCase(31, 2, 13, "The Date of birth must be entered")]
        [TestCase(5, null, 1998, "The Date of birth must be entered")]
        [TestCase(5, 9, null, "The Date of birth must be entered")]
        [TestCase(null, 9, 1998, "The Date of birth must be entered")]
        [TestCase(5, 9, -1, "The Date of birth must be entered")]
        [TestCase(0, 0,  0, "The Date of birth must be entered")]
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

        #endregion

        #region StartDate

        [TestCase(31, 2, 2121, "The Learning start date is not valid")]
        [TestCase(5, null, 2121, "The Learning start date is not valid")]
        [TestCase(5, 9, null, "The Learning start date is not valid")]
        [TestCase(5, 9, -1, "The Learning start date is not valid")]
        [TestCase(0, 0, 0, "The Learning start date is not valid")]
        [TestCase(1, 18, 2121, "The Learning start date is not valid")]
        //[TestCase(5, 9, 1998, "Learner start date must be in the future")]
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

        #endregion

        #region PlannedEndDate

        [TestCase(31, 2, 2121, "The Learning planned end date is not valid")]
        [TestCase(5, null, 2121, "The Learning planned end date is not valid")]
        [TestCase(5, 9, null, "The Learning planned end date is not valid")]
        [TestCase(5, 9, -1, "The Learning planned end date is not valid")]
        [TestCase(0, 0, 0, "The Learning planned end date is not valid")]
        [TestCase(1, 18, 2121, "The Learning planned end date is not valid")]
        [TestCase(5, 9, 1998, "The Learning planned end date must not be in the past")]
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
        public void ShouldNotFailValidationForPlannedEndDate(int? day, int? month, int? year)
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
            result.Errors[0].ErrorMessage.Should().Be("The Learning planned end date must not be in the past");
        }

        [Test]

        public void ShouldFailIfStartDateIsAfterEndDate()
        {
            _validModel.StartDate = new DateTimeViewModel(DateTime.Parse("2121-05-10"));
            _validModel.EndDate = new DateTimeViewModel(DateTime.Parse("2120-05-10"));

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be("The Learning planned end date must not be on or before the Learning start date");
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
