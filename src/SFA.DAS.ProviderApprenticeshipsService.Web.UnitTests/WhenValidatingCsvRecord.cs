using FluentAssertions;

using NUnit.Framework;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests
{
    public class WhenValidatingCsvRecord
    {
        private readonly CsvRecordValidator _validator = new CsvRecordValidator();
        private CsvRecord _validModel;

        [SetUp]
        public void Setup()
        {
            _validModel = new CsvRecord();
        }

        [TestCase(1, "Prog type must be any one of 2, 3, 20, 21, 22, 23, 25", "ProgType_02")]
        [TestCase(99, "Prog type must be any one of 2, 3, 20, 21, 22, 23, 25", "ProgType_02")]
        [TestCase(-1, "Prog type must be any one of 2, 3, 20, 21, 22, 23, 25", "ProgType_02")]
        [TestCase(100, "Cannot be greater than 99", "ProgType_01")]
        [TestCase(null, "Prog type cannot be empty", "ProgType_01")]
        public void ProgTypeValidationFail(int? progType, string message, string errorCode)
        {
            _validModel.ProgType = progType;
            var result = _validator.Validate(_validModel);

            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(message);
            result.Errors[0].ErrorCode.Should().Be(errorCode);
        }

        [TestCase(2, 30, 10, null)]
        [TestCase(3, 30, 10, null)]
        [TestCase(20, 30, 10, null)]
        [TestCase(21, 30, 10, null)]
        [TestCase(22, 30, 10, null)]
        [TestCase(23, 30, 10, null)]
        [TestCase(25, null, null, 42)]
        [TestCase(25, null, null, 42)]
        public void ProgTypeValidationSuccess(int? progType, int? frameworkCode, int? pathwayCode, int? standardCode)
        {
            _validModel.ProgType = progType;
            _validModel.StdCode = standardCode;
            _validModel.FworkCode = frameworkCode;
            _validModel.PwayCode = pathwayCode;

            var result = _validator.Validate(_validModel);
            result.IsValid.Should().BeTrue();
        }

        [TestCase(22, 1000, 10, null, "Framework code must be less than 1000 characters", "FworkCode_01")]
        [TestCase(23, null, 10, null, "Framework code must be greater then 0 for this prog type", "FworkCode_02")]
        [TestCase(23, -1, 10, null, "Framework code must be greater then 0 for this prog type", "FworkCode_02")]
        [TestCase(3, null, null, null, "Framework code must be greater then 0 for this prog type", "FworkCode_02")]
        [TestCase(25, 8, 10, 20, "Framework code must be empty when prog type is 25", "FworkCode_03")]
        public void FrameworkCodeValidationFail(int? progType, int? frameworkCode, int? pathwayCode, int? standardCode, string message, string errorCode)
        {
            _validModel.ProgType = progType;
            _validModel.FworkCode = frameworkCode;
            _validModel.PwayCode = pathwayCode;
            _validModel.StdCode = standardCode;

            var result = _validator.Validate(_validModel);
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(message);
            result.Errors[0].ErrorCode.Should().Be(errorCode);
        }

        [TestCase(22, 10, 1000, null, "Pathway code must be less than 1000 characters", "PwayCode_01")]
        [TestCase(23, 10, null, null, "Pathway code must be greater then 0 for this prog type", "PwayCode_02")]
        [TestCase(23, 10, -1, null, "Pathway code must be greater then 0 for this prog type", "PwayCode_02")]
        //[TestCase(3, null, null, null, "Pathway code must be greater then 0 for this prog type", "FworkCode_02")]        
        public void PathwayCodeValidationFail(int? progType, int? frameworkCode, int? pathwayCode, int? standardCode, string message, string errorCode)
        {
            _validModel.ProgType = progType;
            _validModel.FworkCode = frameworkCode;
            _validModel.PwayCode = pathwayCode;
            _validModel.StdCode = standardCode;

            var result = _validator.Validate(_validModel);
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(message);
            result.Errors[0].ErrorCode.Should().Be(errorCode);
        }

        [Test]
        public void FrameworkAndPathwayValidationErrorWhenProgTypeIs25()
        {
            // [TestCase(25, 8, 10, 20, "Pathway code must be empty when prog type is 25", "PwayCode_03")]
            _validModel.ProgType = 25;
            _validModel.FworkCode = 8;
            _validModel.PwayCode = 10;
            _validModel.StdCode = 20;

            var result = _validator.Validate(_validModel);
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors[0].ErrorMessage.Should().Be("Framework code must be empty when prog type is 25");
            result.Errors[0].ErrorCode.Should().Be("FworkCode_03");

            result.Errors[1].ErrorMessage.Should().Be("Pathway code must be empty when prog type is 25");
            result.Errors[1].ErrorCode.Should().Be("PwayCode_03");
        }

        [TestCase(25, null, null, 100000, "Standard code must be less than 100000 characters", "StdCode_01")]
        [TestCase(25, null, null, -1, "Standard code must be greater then 0 for this prog type", "StdCode_02")]
        [TestCase(25, null, null, null, "Standard code must be greater then 0 for this prog type", "StdCode_02")]
        [TestCase(23, 42, 18, 42, "Standard code must be empty for this prog type", "StdCode_03")]
        public void StandardCodeValidationFail(int? progType, int? frameworkCode, int? pathwayCode, int? standardCode, string message, string errorCode)
        {
            _validModel.ProgType = progType;
            _validModel.FworkCode = frameworkCode;
            _validModel.PwayCode = pathwayCode;
            _validModel.StdCode = standardCode;

            var result = _validator.Validate(_validModel);
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(message);
            result.Errors[0].ErrorCode.Should().Be(errorCode);
        }
    }
}
