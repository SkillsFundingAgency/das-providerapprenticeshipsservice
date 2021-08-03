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

        [TestCase("100000", "The <strong>Standard code</strong> must be 5 characters or fewer", "StdCode_01")]
        [TestCase("-1", "You must enter a <strong>Standard code</strong> - you can add up to 5 characters", "StdCode_02")]
        [TestCase(null, "You must enter a <strong>Standard code</strong>", "StdCode_04")]        
        [TestCase("abba", "The <strong>Standard code</strong> must be 5 characters or fewer", "StdCode_01")]
        public void StandardCodeValidationFail(string standardCode, string message, string errorCode)
        {  
            _validModel.CsvRecord.StdCode = standardCode;

            var result = _validator.Validate(_validModel);
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(message);
            result.Errors[0].ErrorCode.Should().Be(errorCode);
        }
    }
}
