using FluentAssertions;

using NUnit.Framework;

using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests
{
    public class WhenValidatingCsvRecord
    {
        private readonly CsvRecordValidator _validator = new CsvRecordValidator(new BulkUploadApprenticeshipValidationText());
        private CsvRecord _validModel;

        [SetUp]
        public void Setup()
        {
            _validModel = new CsvRecord { CohortRef = "abba123" };
        }

        [TestCase(1, "The <strong>Programme type</strong> is not a valid value", "ProgType_02")]
        [TestCase(99, "The <strong>Programme type</strong> is not a valid value", "ProgType_02")]
        [TestCase(-1, "The <strong>Programme type</strong> is not a valid value", "ProgType_02")]
        [TestCase(100, "The <strong>Programme type</strong> must be entered and must not be more than 2 characters in length", "ProgType_01")]
        [TestCase(null, "The <strong>Programme type</strong> must be entered and must not be more than 2 characters in length", "ProgType_01")]
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

        [TestCase(22, 1000, 10, null, "The <strong>Framework code</strong> must not be more than 3 characters in length", "FworkCode_01")]
        [TestCase(23, null, 10, null, "The <strong>Framework code</strong> must be entered where the Programme Type is a framework", "FworkCode_02")]
        [TestCase(23, -1, 10, null, "The <strong>Framework code</strong> must be entered where the Programme Type is a framework", "FworkCode_02")]
        [TestCase(3, null, null, null, "The <strong>Framework code</strong> must be entered where the Programme Type is a framework", "FworkCode_02")]
        [TestCase(25, 8, 10, 20, "The <strong>Framework code</strong> must not be entered where the Programme Type is a standard", "FworkCode_03")]
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

        [TestCase(22, 10, 1000, null, "The <strong>Pathway code</strong> must not be more than 3 characters in length", "PwayCode_01")]
        [TestCase(23, 10, null, null, "The <strong>Pathway code</strong> must be entered where the Programme Type is a framework", "PwayCode_02")]
        [TestCase(23, 10, -1, null, "The <strong>Pathway code</strong> must be entered where the Programme Type is a framework", "PwayCode_02")]
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
            _validModel.ProgType = 25;
            _validModel.FworkCode = 8;
            _validModel.PwayCode = 10;
            _validModel.StdCode = 20;

            var result = _validator.Validate(_validModel);
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(2);
            result.Errors[0].ErrorMessage.Should().Be("The <strong>Framework code</strong> must not be entered where the Programme Type is a standard");
            result.Errors[0].ErrorCode.Should().Be("FworkCode_03");

            result.Errors[1].ErrorMessage.Should().Be("The <strong>Pathway code</strong> must not be entered where the Programme Type is a standard");
            result.Errors[1].ErrorCode.Should().Be("PwayCode_03");
        }

        [TestCase(25, null, null, 100000, "The <strong>Standard code</strong> must not be more than 5 characters in length", "StdCode_01")]
        [TestCase(25, null, null, -1, "The <strong>Standard code</strong> must be entered where the Programme Type is a standard", "StdCode_02")]
        [TestCase(25, null, null, null, "The <strong>Standard code</strong> must be entered where the Programme Type is a standard", "StdCode_02")]
        [TestCase(23, 42, 18, 42, "The <strong>Standard code</strong> must not be entered where the Programme Type is a framework", "StdCode_03")]
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
