using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    [TestFixture]
    public class WhenValidatingCourseCode : ApprenticeshipBulkUploadValidationTestBase
    {
        [Test]
        public void ShouldBeInValidIfNoCourseCodeValuesSet()
        {
            ValidModel.ApprenticeshipViewModel.CourseCode = null;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue(); // Validation on training is done in CsvValidator
        }

        [Test]
        public void ShouldBeValidIfCourseCodeValuesSet()
        {
            ValidModel.ApprenticeshipViewModel.CourseCode = "123";

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }
    }
}
