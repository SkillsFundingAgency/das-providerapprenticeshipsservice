using Moq;
using SFA.DAS.Learners.Validators;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{
    public class BulkUploadTestHelper
    {
        public static BulkUploadValidator GetBulkUploadValidator(int maxBulkUploadFileSize = 0)
        {
            return new BulkUploadValidator(new ProviderApprenticeshipsServiceConfiguration { MaxBulkUploadFileSize = maxBulkUploadFileSize },
                                                Mock.Of<IUlnValidator>(),
                                                Mock.Of<IAcademicYearDateProvider>());
        }
    }
}
