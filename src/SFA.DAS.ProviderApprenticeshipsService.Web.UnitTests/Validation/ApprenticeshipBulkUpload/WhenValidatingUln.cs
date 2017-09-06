using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Learners.Validators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    [TestFixture]
    public class WhenValidatingUln : ApprenticeshipBulkUploadValidationTestBase
    {
        [Test]
        public void ShouldCallUlnValidatorService()
        {
            ValidModel.ApprenticeshipViewModel.ULN = "123456789";

            MockUlnValidator
                .Setup(m => m.Validate(ValidModel.ApprenticeshipViewModel.ULN))
                .Returns(UlnValidationResult.IsInValidTenDigitUlnNumber);

           Validator.Validate(ValidModel);

            MockUlnValidator
               .Verify(m => m.Validate(ValidModel.ApprenticeshipViewModel.ULN), Times.AtLeastOnce);
        }

        [Test]
        public void ShouldBeInvalidIfResultIsNotSuccess()
        {
            ValidModel.ApprenticeshipViewModel.ULN = "123456789";

            MockUlnValidator
                .Setup(m => m.Validate(ValidModel.ApprenticeshipViewModel.ULN))
                .Returns(UlnValidationResult.IsInValidTenDigitUlnNumber);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ShouldBeValidIfResultIsSuccess()
        {
            ValidModel.ApprenticeshipViewModel.ULN = "1748529632";

            MockUlnValidator
             .Setup(m => m.Validate(ValidModel.ApprenticeshipViewModel.ULN))
             .Returns(UlnValidationResult.Success);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }
    }
}
