﻿namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class CohortsViewModel : ViewModelBase
    {
        public int NewRequestsCount { get; set; }

        public int ReadyForApprovalCount { get; set; }

        public int ReadyForReviewCount { get; set; }

        public int WithEmployerCount { get; set; }

        public bool HasSignedTheAgreement { get; set; }

        public string SignAgreementUrl { get; set; }
    }
}