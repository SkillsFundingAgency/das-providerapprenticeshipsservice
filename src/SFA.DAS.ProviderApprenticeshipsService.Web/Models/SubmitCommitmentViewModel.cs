using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{

    public class SubmitCommitmentViewModel
    {
        public long ProviderId { get; set; }

        public string HashedCommitmentId { get; set; }

        public string Message { get; set; }

        public string EmployerName { get; internal set; }

        public SaveStatus SaveStatus { get; set; }
    }
}