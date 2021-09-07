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

        [TestCase(190, 0)]        
        [TestCase(201, 1)]
        public void TestEmailAddressNoLongerThan200Characters(int length, int expectedErrorCount)
        {
            //Arrange            
            ValidModel.CsvRecord.EmailAddress = new string('*', length) + "@test.com";           

            //Act
            var result = Validator.Validate(ValidModel);
            
            //Assert
            result.Errors.Count.Should().Be(expectedErrorCount);
            if (expectedErrorCount > 0)
            {
                result.Errors[0].ErrorMessage.Should().Be("You must enter an <strong>email address</strong> that’s no longer than 200 characters");
            }
        }

        [Test]
        public void TestBlackListValidateEmailAddressIfProvided()
        {
            //Arrange
            ValidModel.ApprenticeshipViewModel.BlackListed = true;
            ValidModel.CsvRecord.EmailAddress = "apprentice1@test.com";

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            result.Errors.Count.Should().Be(0);            
        }

        [Test]
        public void TestBlackListNullEmailAddress()
        {
            //Arrange
            ValidModel.ApprenticeshipViewModel.BlackListed = true;
            ValidModel.CsvRecord.EmailAddress = null;

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            result.Errors.Count.Should().Be(0);
        }

        [Test]
        public void TestBlackListInvalidEmail()
        {
            //Arrange
            ValidModel.ApprenticeshipViewModel.BlackListed = true;
            ValidModel.CsvRecord.EmailAddress = "apprentice@test";

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            result.Errors.Count.Should().Be(1);
            result.Errors[0].ErrorMessage.Should().Be("You must enter a valid <strong>email address</strong>");
        }

        [TestCase(190, 0)]
        [TestCase(201, 1)]
        public void TestBlackListEmailAddressNoLongerThan200Characters(int length, int expectedErrorCount)
        {
            //Arrange
            ValidModel.ApprenticeshipViewModel.BlackListed = true;
            ValidModel.CsvRecord.EmailAddress = new string('*', length) + "@test.com";

            //Act
            var result = Validator.Validate(ValidModel);

            //Assert
            result.Errors.Count.Should().Be(expectedErrorCount);
            if (expectedErrorCount > 0)
            {
                result.Errors[0].ErrorMessage.Should().Be("You must enter an <strong>email address</strong> that’s no longer than 200 characters");
            }
        }
    }
}
