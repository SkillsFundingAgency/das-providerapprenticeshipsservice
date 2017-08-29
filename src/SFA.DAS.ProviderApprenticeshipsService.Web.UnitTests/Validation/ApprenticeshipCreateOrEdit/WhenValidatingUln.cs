using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Learners.Validators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingUln : ApprenticeshipValidationTestBase
    {

        [Test]
        public void ShouldCallUlnValidatorService()
        {
            ValidModel.ULN = "123456789";

            MockUlnValidator
                .Setup(m => m.Validate(ValidModel.ULN))
                .Returns(UlnValidationResult.IsInValidTenDigitUlnNumber);

            Validator.Validate(ValidModel);

            MockUlnValidator
               .Verify(m => m.Validate(ValidModel.ULN), Times.AtLeastOnce);
        }

        [Test]
        public void ShouldBeInvalidIfResultIsNotSuccess()
        {
            ValidModel.ULN = "123456789";

            MockUlnValidator
                .Setup(m => m.Validate(ValidModel.ULN))
                .Returns(UlnValidationResult.IsInValidTenDigitUlnNumber);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void ShouldBeValidIfResultIsSuccess()
        {
            ValidModel.ULN = "1748529632";

            MockUlnValidator
             .Setup(m => m.Validate(ValidModel.ULN))
             .Returns(UlnValidationResult.Success);

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

    }
}
