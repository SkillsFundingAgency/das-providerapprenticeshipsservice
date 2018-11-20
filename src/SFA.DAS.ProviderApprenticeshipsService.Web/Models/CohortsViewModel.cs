namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class CohortsViewModel : ViewModelBase
    {
        public int ReadyForReviewCount { get; set; }

        public int WithEmployerCount { get; set; }

        public int DraftCount { get; set; }

        public int? TransferFundedCohortsCount { get; set; }

        public bool HasSignedTheAgreement { get; set; }

        public string SignAgreementUrl { get; set; }
        public bool ShowDrafts { get; set; }
    }
}