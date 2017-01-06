using System.Web;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload
{
    public interface IBulkUploadFileParser
    {
        BulkUploadResult CreateViewModels(HttpPostedFileBase attachment);
    }
}
