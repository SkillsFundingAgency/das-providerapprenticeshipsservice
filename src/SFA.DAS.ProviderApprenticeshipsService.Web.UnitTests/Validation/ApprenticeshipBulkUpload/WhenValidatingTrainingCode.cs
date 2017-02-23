using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    [TestFixture]
    public class WhenValidatingTrainingCode : ApprenticeshipBulkUploadValidationTestBase
    {
        [Test]
        public void ShouldBeInValidIfNoTrainingCodeValuesSet()
        {
            ValidModel.TrainingCode = null;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue(); // Validation on training is done in CsvValidator
        }

        [Test]
        public void ShouldBeValidIfTrainingCodeValuesSet()
        {
            ValidModel.TrainingCode = "123";

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }
    }
}
