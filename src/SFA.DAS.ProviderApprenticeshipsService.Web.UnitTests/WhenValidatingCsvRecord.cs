using System;
using FluentAssertions;

using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;
using SFA.DAS.Learners.Validators;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using Moq;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests
{
    public class WhenValidatingCsvRecord
    {
        private ApprenticeshipUploadModelValidator _validator;
        private ApprenticeshipUploadModel _validModel;
        private Moq.Mock<IUlnValidator> _mockUlnValidator;
        private Moq.Mock<IAcademicYearDateProvider> _mockAcademicYear;

        [SetUp]
        public void Setup()
        {
            var now = DateTime.UtcNow;
            _validModel = new ApprenticeshipUploadModel
            {
                ApprenticeshipViewModel = new ApprenticeshipViewModel
                {
                    ULN = "1001234567",
                    FirstName = "TestFirstName",
                    LastName = "TestLastName",
                    CourseCode = "12",
                    DateOfBirth = new DateTimeViewModel(now.AddYears(-16)),
                    StartDate = new DateTimeViewModel(now),
                    EndDate = new DateTimeViewModel(now.AddYears(3)),
                    Cost = "1234"                    
                },
                CsvRecord = new CsvRecord { CohortRef = "abba123", EmailAddress = "apprentice1@test.com", AgreementId = "XYZUR" }
            };

            _mockAcademicYear = new Moq.Mock<IAcademicYearDateProvider>();
            _mockUlnValidator = new Moq.Mock<IUlnValidator>();
            _mockUlnValidator.Setup(m => m.Validate(_validModel.ApprenticeshipViewModel.ULN)).Returns(UlnValidationResult.Success);
            _mockAcademicYear.Setup(m => m.CurrentAcademicYearEndDate).Returns(new DateTime(2030, 12, 1));

            _validator = new ApprenticeshipUploadModelValidator(new BulkUploadApprenticeshipValidationText(_mockAcademicYear.Object), new CurrentDateTime(), _mockUlnValidator.Object, _mockAcademicYear.Object);
        }

        //[TestCase("1", "The <strong>Programme type</strong> you've added isn't valid", "ProgType_02")]
        //[TestCase("99", "The <strong>Programme type</strong> you've added isn't valid", "ProgType_02")]
        //[TestCase("-1", "The <strong>Programme type</strong> you've added isn't valid", "ProgType_02")]
        //[TestCase("100", "You must enter a <strong>Programme type</strong> - you can add up to 2 characters", "ProgType_01")]
        //[TestCase(null, "You must enter a <strong>Programme type</strong> - you can add up to 2 characters", "ProgType_01")]
        //[TestCase("abba123", "You must enter a <strong>Programme type</strong> - you can add up to 2 characters", "ProgType_01")]
        //public void ProgTypeValidationFail(string progType, string message, string errorCode)
        //{
        //    _validModel.CsvRecord.ProgType = progType;
        //    var result = _validator.Validate(_validModel);

        //    result.IsValid.Should().BeFalse();
        //    result.Errors[0].ErrorMessage.Should().Be(message);
        //    result.Errors[0].ErrorCode.Should().Be(errorCode);
        //}

        //[TestCase("2", "30", "10", null)]
        //[TestCase("3", "30", "10", null)]
        //[TestCase("20", "30", "10", null)]
        //[TestCase("21", "30", "10", null)]
        //[TestCase("22", "30", "10", null)]
        //[TestCase("23", "30", "10", null)]
        //[TestCase("25", null, null, "42")]
        //public void ProgTypeValidationSuccess(string progType, string frameworkCode, string pathwayCode, string standardCode)
        //{
        //    _validModel.CsvRecord.ProgType = progType;
        //    _validModel.CsvRecord.StdCode = standardCode;
        //    _validModel.CsvRecord.FworkCode = frameworkCode;
        //    _validModel.CsvRecord.PwayCode = pathwayCode;

        //    var result = _validator.Validate(_validModel);
        //    result.IsValid.Should().BeTrue();
        //}

        //[TestCase("22", "1000", "10", null, "The <strong>Framework code</strong> must be 3 characters or fewer", "FworkCode_01")]
        //[TestCase("23", null, "10", null, "You must enter a <strong>Framework code</strong> - you can add up to 3 characters", "FworkCode_02")]
        //[TestCase("23", "-1", "10", null, "You must enter a <strong>Framework code</strong> - you can add up to 3 characters", "FworkCode_02")]
        //[TestCase("3", null, null, null, "You must enter a <strong>Framework code</strong> - you can add up to 3 characters", "FworkCode_02")]
        //[TestCase("25", "8", "10", "20", "You must not enter a <strong>Framework code</strong> when you've entered a Standard programme type", "FworkCode_03")]
        //[TestCase("23", "ab3", "10", null, "The <strong>Framework code</strong> must be 3 characters or fewer", "FworkCode_01")]
        //public void FrameworkCodeValidationFail(string progType, string frameworkCode, string pathwayCode, string standardCode, string message, string errorCode)
        //{
        //    _validModel.CsvRecord.ProgType = progType;
        //    _validModel.CsvRecord.FworkCode = frameworkCode;
        //    _validModel.CsvRecord.PwayCode = pathwayCode;
        //    _validModel.CsvRecord.StdCode = standardCode;

        //    var result = _validator.Validate(_validModel);
        //    result.IsValid.Should().BeFalse();
        //    result.Errors[0].ErrorMessage.Should().Be(message);
        //    result.Errors[0].ErrorCode.Should().Be(errorCode);
        //}

        //[TestCase("22", "10", "1000", null, "The <strong>Pathway code</strong> must be 3 characters or fewer", "PwayCode_01")]
        //[TestCase("23", "10", null, null, "You must enter a <strong>Pathway code</strong> = you can add up to 3 characters", "PwayCode_02")]
        //[TestCase("23", "10", "-1", null, "You must enter a <strong>Pathway code</strong> = you can add up to 3 characters", "PwayCode_02")]
        //public void PathwayCodeValidationFail(string progType, string frameworkCode, string pathwayCode, string standardCode, string message, string errorCode)
        //{
        //    _validModel.CsvRecord.ProgType = progType;
        //    _validModel.CsvRecord.FworkCode = frameworkCode;
        //    _validModel.CsvRecord.PwayCode = pathwayCode;
        //    _validModel.CsvRecord.StdCode = standardCode;

        //    var result = _validator.Validate(_validModel);
        //    result.IsValid.Should().BeFalse();
        //    result.Errors[0].ErrorMessage.Should().Be(message);
        //    result.Errors[0].ErrorCode.Should().Be(errorCode);
        //}

        //[Test]
        //public void FrameworkAndPathwayValidationErrorWhenProgTypeIs25()
        //{
        //    _validModel.CsvRecord.ProgType = "25";
        //    _validModel.CsvRecord.FworkCode = "8";
        //    _validModel.CsvRecord.PwayCode = "10";
        //    _validModel.CsvRecord.StdCode = "20";

        //    var result = _validator.Validate(_validModel);
        //    result.IsValid.Should().BeFalse();
        //    result.Errors.Count.Should().Be(2);
        //    result.Errors[0].ErrorMessage.Should().Be("You must not enter a <strong>Framework code</strong> when you've entered a Standard programme type");
        //    result.Errors[0].ErrorCode.Should().Be("FworkCode_03");

        //    result.Errors[1].ErrorMessage.Should().Be("You must not enter a <strong>Pathway code</strong> when you've entered a Standard programme type");
        //    result.Errors[1].ErrorCode.Should().Be("PwayCode_03");
        //}

        [TestCase("100000", "The <strong>Standard code</strong> must be 5 characters or fewer", "StdCode_01")]
        [TestCase("-1", "You must enter a <strong>Standard code</strong> - you can add up to 5 characters", "StdCode_02")]
        [TestCase(null, "You must enter a <strong>Standard code</strong>", "StdCode_04")]
        //[TestCase("42", "You must not enter a <strong>Standard code</strong> when you've entered a Framework programme type", "StdCode_03")]
        [TestCase("abba", "The <strong>Standard code</strong> must be 5 characters or fewer", "StdCode_01")]
        public void StandardCodeValidationFail(string standardCode, string message, string errorCode)
        {
            //_validModel.CsvRecord.ProgType = progType;
            //_validModel.CsvRecord.FworkCode = frameworkCode;
            //_validModel.CsvRecord.PwayCode = pathwayCode;
            _validModel.CsvRecord.StdCode = standardCode;

            var result = _validator.Validate(_validModel);
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(message);
            result.Errors[0].ErrorCode.Should().Be(errorCode);
        }
    }
}
