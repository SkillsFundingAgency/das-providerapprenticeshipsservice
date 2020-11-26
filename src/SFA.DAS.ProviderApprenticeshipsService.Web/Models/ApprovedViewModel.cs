namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ApprovedViewModel
    {
        public string Headline { get; internal set; }
        public string CommitmentReference { get; internal set; }
        public string EmployerName { get; internal set; }
        public string ProviderName { get; internal set; }
        public bool HasOtherCohortsAwaitingApproval { get; internal set; }
        public bool IsTransfer { get; internal set; }
        public long? ChangeOfPartyRequestId { get; internal set; }
    }
}