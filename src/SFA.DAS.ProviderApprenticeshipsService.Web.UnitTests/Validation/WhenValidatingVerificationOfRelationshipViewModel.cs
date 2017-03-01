using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation
{
    [TestFixture]
    public class WhenValidatingVerificationOfRelationshipViewModel
    {
        private VerificationOfRelationshipViewModelValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new VerificationOfRelationshipViewModelValidator();
        }

        [Test]
        public void ThenAnOptionMustBeSelected()
        {
            //Arrange
            var viewModel = new VerificationOfRelationshipViewModel();

            //Act
            var result = _validator.Validate(viewModel);

            //Assert
            Assert.IsFalse(result.IsValid);
        }
    }
}
