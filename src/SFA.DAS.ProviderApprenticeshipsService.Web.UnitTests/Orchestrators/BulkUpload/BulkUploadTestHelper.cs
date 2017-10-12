using Moq;
using SFA.DAS.Learners.Validators;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{
    public class BulkUploadTestHelper
    {
        public static BulkUploadValidator GetBulkUploadValidator(int maxBulkUploadFileSize = 0)
        {
          var  validationText = new BulkUploadApprenticeshipValidationText(Mock.Of<IAcademicYearDateProvider>());
          var  viewModelValidator = new ApprenticeshipUploadModelValidator(validationText, new CurrentDateTime(), Mock.Of<IUlnValidator>());

            return new BulkUploadValidator(new ProviderApprenticeshipsServiceConfiguration { MaxBulkUploadFileSize = maxBulkUploadFileSize },
                                                Mock.Of<ILog>(),
                                                Mock.Of<IUlnValidator>(),
                                                Mock.Of<IAcademicYearDateProvider>());
        }

    }
}
