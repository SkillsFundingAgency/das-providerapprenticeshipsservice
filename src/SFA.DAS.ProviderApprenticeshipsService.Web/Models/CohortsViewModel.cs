namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class CohortsViewModel
    {
        public int NewRequestsCount { get; set; }

        public int ReadyForApprovalCount { get; set; }

        public int ReadyForReviewCount { get; set; }

        public int WithEmployerCount { get; set; }
    }
}