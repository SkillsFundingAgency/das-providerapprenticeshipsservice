namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

    public class SubmitCommitmentViewModel
    {
        public long ProviderId { get; set; }
        public long CommitmentId { get; set; }
        public string Message { get; set; }

        public string EmployerName { get; internal set; }

        public SaveStatus SaveStatus { get; set; }
    }
}