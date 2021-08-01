using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    [TestFixture]
    public class WhenValidatingEmail : ApprenticeshipBulkUploadValidationTestBase
    {
        [Test]
        public void TestEmailAddressNotNull()
        {
            //Arrange
            ValidModel.CsvRecord.EmailAddress = null;

            //Act
            var result = Validator.Validate(ValidModel);
            
            //Assert
            result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.Should().Be("<strong>Email address</strong> must be entered");
        }

        [TestCase("apprentice")]
        [TestCase("apprentice@")]
        [TestCase("apprentice@test")]
        [TestCase(".@test.com")]        
        public void TestEmailAddressNotValid(string email)
        {
            //Arrange
            ValidModel.CsvRecord.EmailAddress = email;

            //Act
            var result = Validator.Validate(ValidModel);
            
            //Assert
            result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.Should().Be("You must enter a valid <strong>email address</strong>");
        }

        [Test]
        public void TestEmailAddressNoLongerThan200Characters()
        {
            //Arrange
            ValidModel.CsvRecord.EmailAddress = "apprentice1apprentice1apprentice1apprentice1apprentice1apprentice1apprentice1apprentice1apprentice1apprentice1apprentice1apprentice1apprentice1apprentice1apprentice1apprentice1apprentice1testlength@test.com";

            //Act
            var result = Validator.Validate(ValidModel);
            
            //Assert
            result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.Should().Be("You must enter an <strong>email address</strong> that’s no longer than 200 characters");
        }
    }
}
