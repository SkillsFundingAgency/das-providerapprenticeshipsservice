namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class AcknowledgementViewModel
    {
        public string CommitmentReference { get; internal set; }
        public string EmployerName { get; internal set; }
        public string Message { get; set; }
        public string ProviderName { get; internal set; }
    }
}