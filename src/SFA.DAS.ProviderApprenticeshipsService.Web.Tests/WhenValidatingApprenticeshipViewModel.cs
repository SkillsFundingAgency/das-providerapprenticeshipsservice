﻿using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Tests
{
    [TestFixture]
    public class WhenValidatingApprenticeshipViewModel
    {
        private ApprenticeshipViewModelValidator _validator = new ApprenticeshipViewModelValidator();

        [Test]
        public void ULNMustBeNumericAnd10DigitsInLength()
        {
            var viewModel = new ApprenticeshipViewModel { ULN = "1001234567" };

            var result = _validator.Validate(viewModel);

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
            var viewModel = new ApprenticeshipViewModel { Cost = cost };

            var result = _validator.Validate(viewModel);

            result.IsValid.Should().BeTrue();
        }

        [TestCase("123.12")]
        [TestCase("123.1")]
        [TestCase("123.0")]
        [TestCase("fdsfdfd")]
        [TestCase("123.000")]
        public void CostNotNumericOrIsNotAWholeNumber(string cost)
        {
            var viewModel = new ApprenticeshipViewModel { Cost = cost };

            var result = _validator.Validate(viewModel);

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
            var viewModel = new ApprenticeshipViewModel { Cost = cost };

            var result = _validator.Validate(viewModel);

            result.IsValid.Should().BeFalse();
        }
    }
}
