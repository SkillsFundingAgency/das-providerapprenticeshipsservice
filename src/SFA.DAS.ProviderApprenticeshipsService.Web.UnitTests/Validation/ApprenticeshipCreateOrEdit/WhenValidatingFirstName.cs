using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingFirstName
    {
        private readonly ApprenticeshipViewModelValidator _validator = new ApprenticeshipViewModelValidator(new WebApprenticeshipValidationText());
        private ApprenticeshipViewModel _validModel;

        [SetUp]
        public void Setup()
        {
            _validModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
        }

        [Test]
        public void TestFirstNamesNotNull()
        {
            _validModel.FirstName = null;

            var result = _validator.Validate(_validModel);
            result.Errors.Count.Should().Be(1);

            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("First name must be entered");
        }

        [Test]
        public void FirstNameShouldNotBeEmpty()
        {
            _validModel.FirstName = " ";

            var result = _validator.Validate(_validModel);
            result.Errors.Count.Should().Be(1);

            result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("First name must be entered");
        }

        [TestCase(99, 0)]
        [TestCase(100, 0)]
        [TestCase(101, 1)]
        public void TestLengthOfFirstName(int length, int expectedErrorCount)
        {
            _validModel.FirstName = new string('*', length);

            var result = _validator.Validate(_validModel);

            result.Errors.Count.Should().Be(expectedErrorCount);

            if (expectedErrorCount > 0)
            {
                result.Errors[0].ErrorMessage.ShouldBeEquivalentTo("You must enter a first name that's no longer than 100 characters");
            }
        }
    }
}