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
    public class WhenValidatingLastName
    {
        private readonly ApprenticeshipViewModelValidator _validator = new ApprenticeshipViewModelValidator(new WebApprenticeshipValidationText());
        private ApprenticeshipViewModel _validModel;

        [SetUp]
        public void Setup()
        {
            _validModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
        }

        [Test]
        public void TestLastNameNotNull()
        {
            _validModel.LastName = null;

            var result = _validator.Validate(_validModel);

            result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("Last name must be entered");
        }

        [Test]
        public void LastNameShouldNotBeEmpty()
        {
            _validModel.LastName = " ";

            var result = _validator.Validate(_validModel);
            result.Errors.Count.Should().Be(1);

            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("Last name must be entered");
        }

        [TestCase(99, 0)]
        [TestCase(100, 0)]
        [TestCase(101, 1)]
        public void TestLengthOfLastName(int length, int expectedErrorCount)
        {
            _validModel.LastName = new string('*', length);

            var result = _validator.Validate(_validModel);

            result.Errors.Count.Should().Be(expectedErrorCount);

            if (expectedErrorCount > 0)
            {
                result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("You must enter a last name that's no longer than 100 characters");
            }
        }
    }
}
