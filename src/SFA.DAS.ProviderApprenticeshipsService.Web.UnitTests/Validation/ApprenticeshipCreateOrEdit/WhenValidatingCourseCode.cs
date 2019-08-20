using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipCreateOrEdit
{
    [TestFixture]
    public class WhenValidatingCourseCode : ApprenticeshipValidationTestBase
    {
        [Test]
        public void ShouldBeValidIfNoCourseCodeValuesSet()
        {
            ValidModel.CourseCode = null;

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }

        [Test]
        public void ShouldBeValidIfCourseCodeValuesSet()
        {
            ValidModel.CourseCode = "123";

            var result = Validator.Validate(ValidModel);

            result.IsValid.Should().BeTrue();
        }
    }
}
