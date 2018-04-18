using SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class CommitmentListItemViewModel
    {
        public string HashedCommitmentId { get; set; }
        public string LegalEntityName { get; set; }
        public string Reference { get; set; }
        public RequestStatus Status { get; set; }
        public string ProviderName { get; set; }

        public string LatestMessage { get; set; }
        public long EmployerAccountId { get; set; }
    }
}