using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload
{
    public interface IBulkUploadFileParser
    {
        BulkUploadResult CreateViewModels(long providerId, CommitmentView commitment, string fileInput, bool blackListed);
    }
}
