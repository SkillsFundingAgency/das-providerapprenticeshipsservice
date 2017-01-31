namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class AgreementNotSignedViewModel
    {
        public long ProviderId { get; set; }

        public string HashedCommitmentId { get; set; }

        public string ReviewAgreementUrl { get; set; }

        public bool IsSignedAgreement { get; set; }

        public string RequestListUrl { get; set; }
    }
}