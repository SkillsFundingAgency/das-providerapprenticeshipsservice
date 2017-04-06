using System.Linq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.ApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipUpdate
{
    [TestFixture]
    public class WhenValidatingUndoApprenticeshipUpdateViewModel
    {
        private UndoApprenticeshipUpdateViewModelValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new UndoApprenticeshipUpdateViewModelValidator();
        }

        [Test]
        public void ThenAnOptionMustBeSelected()
        {
            //Arrange
            var viewModel = new UndoApprenticeshipUpdateViewModel();

            //Act
            var result = _validator.Validate(viewModel);

            //Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName == nameof(UndoApprenticeshipUpdateViewModel.ConfirmUndo)));
        }
    }
}
