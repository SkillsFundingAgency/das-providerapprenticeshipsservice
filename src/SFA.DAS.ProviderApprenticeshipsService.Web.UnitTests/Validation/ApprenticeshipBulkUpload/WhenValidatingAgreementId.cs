using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    [TestFixture]
    public class WhenValidatingAgreementId : ApprenticeshipBulkUploadValidationTestBase
    {
        [Test]
        public void TestAgreementIdNotNull()
        {
            //Arrange
            ValidModel.CsvRecord.AgreementId = null;

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.Should().Be("<strong>Agreement ID</strong> must be entered");
        }
    }
}
