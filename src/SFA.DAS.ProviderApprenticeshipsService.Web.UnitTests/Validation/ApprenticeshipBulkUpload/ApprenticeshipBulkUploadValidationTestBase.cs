using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Validation.ApprenticeshipBulkUpload
{
    public abstract class ApprenticeshipBulkUploadValidationTestBase
    {
        protected ApprenticeshipBulkUploadValidator Validator = new ApprenticeshipBulkUploadValidator(new BulkUploadApprenticeshipValidationText(), new CurrentDateTime());
        protected ApprenticeshipViewModel ValidModel;
        protected Mock<ICurrentDateTime> MockCurrentDateTime;

        [SetUp]
        public void BaseSetup()
        {
            MockCurrentDateTime = new Mock<ICurrentDateTime>();
            Validator = new ApprenticeshipBulkUploadValidator(new BulkUploadApprenticeshipValidationText(), MockCurrentDateTime.Object);

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
