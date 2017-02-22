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
    public class WhenValidatingCost
    {
        private readonly ApprenticeshipViewModelValidator _validator = new ApprenticeshipViewModelValidator(new WebApprenticeshipValidationText());
        private ApprenticeshipViewModel _validModel;

        [SetUp]
        public void Setup()
        {
            _validModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
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

        [TestCase("100,000")]
        public void CostMustContain6DigitsOrLessIgnoringCommas(string value)
        {
            _validModel.Cost = value;

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        public void CostContainingValidCommaSeparatorIsValid()
        {
            _validModel.Cost = "1,234";

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeTrue();
        }

        [TestCase(",111")]
        [TestCase("1,22")]
        [TestCase("122,22")]
        [TestCase("12222,")]
        public void CostThatContainsBadlyFormatedCommaSeparatorsIsInvalid(string cost)
        {
            _validModel.Cost = cost;

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void CostCannotBeOver100000()
        {
            _validModel.Cost = "100001";

            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
        }
    }
}
