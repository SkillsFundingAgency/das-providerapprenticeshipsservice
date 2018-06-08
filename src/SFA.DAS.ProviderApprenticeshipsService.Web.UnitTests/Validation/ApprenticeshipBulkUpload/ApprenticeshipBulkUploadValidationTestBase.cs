using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;
using SFA.DAS.Learners.Validators;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    public abstract class ApprenticeshipBulkUploadValidationTestBase
    {
        protected ApprenticeshipUploadModelValidator Validator;
        protected ApprenticeshipUploadModel ValidModel;
        protected Mock<IUlnValidator> MockUlnValidator;
        protected Mock<IAcademicYearDateProvider> MockAcademicYear;
        protected Mock<IAcademicYearValidator> MockAcademicYearValidator;

        [SetUp]
        public void BaseSetup()
        {
            MockUlnValidator = new Mock<IUlnValidator>();
            MockAcademicYear = new Mock<IAcademicYearDateProvider>();
            MockAcademicYearValidator = new Mock<IAcademicYearValidator>();

            Validator = new ApprenticeshipUploadModelValidator(new BulkUploadApprenticeshipValidationText(MockAcademicYear.Object), MockUlnValidator.Object);

            ValidModel = new ApprenticeshipUploadModel
            {
                ApprenticeshipViewModel = new ApprenticeshipViewModel
                {
                    ULN = "1001234567",
                    FirstName = "TestFirstName",
                    LastName = "TestLastName",
                    TrainingCode = "12",
                    DateOfBirth = new DateTimeViewModel(DateTime.UtcNow.AddYears(-18)),
                    StartDate = new DateTimeViewModel(new DateTime(2017, 06, 20)),
                    EndDate = new DateTimeViewModel(new DateTime(2018, 05, 15)),
                    Cost = "1234"
                },
                CsvRecord = new CsvRecord { CohortRef = "abba123", ProgType = "25", StdCode = "123" }
            };
        }
    }
}
