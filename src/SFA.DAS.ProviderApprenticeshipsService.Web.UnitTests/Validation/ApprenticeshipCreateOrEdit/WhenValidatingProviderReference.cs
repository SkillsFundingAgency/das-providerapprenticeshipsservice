using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingProviderReference
    {
        private readonly ApprenticeshipViewModelValidator _validator = new ApprenticeshipViewModelValidator(new WebApprenticeshipValidationText());
        private ApprenticeshipViewModel _validModel;

        [SetUp]
        public void Setup()
        {
            _validModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
        }

        [TestCase("")]
        [TestCase(null)]
        public void ProviderReferenceIsOptional(string reference)
        {
            _validModel.ProviderRef = reference;

            var result = _validator.Validate(_validModel);

            result.Errors.Count.Should().Be(0);
        }

        [Test]
        public void ProviderReferenceCanBeAMaximumOf20Characters()
        {
            _validModel.ProviderRef = new string('*', 21);

            var result = _validator.Validate(_validModel);

            result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.Should().Be("The Reference must be 20 characters or fewer");
            result.Errors[0].ErrorCode.Should().Be("ProviderRef_01");
        }

        [Test]
        public void ProviderReferenceLessCanBeLessThan20Characters()
        {
            _validModel.ProviderRef = "A valid reference";

            var result = _validator.Validate(_validModel);

            result.Errors.Count.Should().Be(0);
        }
    }
}
