using SFA.DAS.Tasks.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class TaskViewModel
    {
        public long ProviderId { get; set; }
        public Task Task { get; set; }
        public long LinkId { get; set; }
        public string HashedCommitmentId { get; set; }

        public string Message { get; set; }

    }
}