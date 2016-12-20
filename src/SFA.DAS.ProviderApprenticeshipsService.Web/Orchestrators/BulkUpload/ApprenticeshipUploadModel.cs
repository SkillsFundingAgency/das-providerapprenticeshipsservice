using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload
{
    public class ApprenticeshipUploadModel
    {
        public ApprenticeshipViewModel ApprenticeshipViewModel { get; set; }

        public CsvRecord CsvRecord { get; set; }
    }
}