namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class SubmitCommitmentViewModel
    {
        public long ProviderId { get; set; }
        public long CommitmentId { get; set; }
        public string Message { get; set; }
        public string SaveOrSend { get; internal set; }
        public string EmployerName { get; internal set; }
    }
}