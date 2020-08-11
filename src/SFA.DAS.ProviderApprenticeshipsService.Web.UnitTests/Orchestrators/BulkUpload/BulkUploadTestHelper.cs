using Moq;
using SFA.DAS.Learners.Validators;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{
    public class BulkUploadTestHelper
    {
        public static BulkUploadValidator GetBulkUploadValidator(int maxBulkUploadFileSize = 0)
        {
            var mockAcademicYear = new Moq.Mock<IAcademicYearDateProvider>();
            mockAcademicYear.Setup(m => m.CurrentAcademicYearEndDate).Returns(new DateTime(2030, 12, 1));
            return new BulkUploadValidator(new ProviderApprenticeshipsServiceConfiguration { MaxBulkUploadFileSize = maxBulkUploadFileSize },
                                                Mock.Of<IUlnValidator>(),
                                                mockAcademicYear.Object);
        }
    }
}
