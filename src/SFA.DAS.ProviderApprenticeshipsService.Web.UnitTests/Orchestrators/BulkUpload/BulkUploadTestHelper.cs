using Moq;
using SFA.DAS.Learners.Validators;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{
    public class BulkUploadTestHelper
    {
        public static BulkUploadValidator GetBulkUploadValidator(int maxBulkUploadFileSize = 0)
        {
            var validationText =
                new BulkUploadApprenticeshipValidationText(new AcademicYearDateProvider(new CurrentDateTime()));

            return new BulkUploadValidator(
                new ProviderApprenticeshipsServiceConfiguration { MaxBulkUploadFileSize = maxBulkUploadFileSize },
                validationText,
                new ApprenticeshipUploadModelValidator(
                    validationText,
                    new CurrentDateTime(),
                    Mock.Of<IUlnValidator>(),
                    Mock.Of<IAcademicYearValidator>()));
        }
    }
}
