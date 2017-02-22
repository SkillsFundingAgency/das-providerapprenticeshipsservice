using System;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    public abstract class ApprenticeshipBulkUploadValidationTestBase
    {
        protected readonly ApprenticeshipBulkUploadValidator Validator = new ApprenticeshipBulkUploadValidator(new BulkUploadApprenticeshipValidationText());
        protected ApprenticeshipViewModel ValidModel;

        [SetUp]
        public void BaseSetup()
        {
            ValidModel = new ApprenticeshipViewModel
            {
                ULN = "1001234567",
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                TrainingCode = "12",
                DateOfBirth = new DateTimeViewModel(DateTime.UtcNow.AddYears(-16)),
                StartDate = new DateTimeViewModel(new DateTime(2017, 06, 20)),
                EndDate = new DateTimeViewModel(new DateTime(2018, 05, 15)),
                Cost = "1234"
            };
        }
    }
}
