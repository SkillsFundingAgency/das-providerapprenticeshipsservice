using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation
{
    [TestFixture]
    public class WhenValidatingVerificationOfEmployerViewModel
    {
        private VerificationOfEmployerViewModelValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new VerificationOfEmployerViewModelValidator();
        }

        [Test]
        public void ThenAnOptionMustBeSelected()
        {
            //Arrange
            var viewModel = new VerificationOfEmployerViewModel();

            //Act
            var result = _validator.Validate(viewModel);

            //Assert
            Assert.IsFalse(result.IsValid);
        }
    }
}
