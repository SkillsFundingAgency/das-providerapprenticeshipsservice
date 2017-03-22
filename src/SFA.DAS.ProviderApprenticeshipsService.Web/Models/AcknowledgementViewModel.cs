using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class AcknowledgementViewModel
    {
        public string CommitmentReference { get; internal set; }
        public string EmployerName { get; internal set; }
        public string Message { get; set; }
        public string ProviderName { get; internal set; }

        public string RedirectUrl { get; set; }

        public string RedirectLinkText { get; set; }

        public string PageTitle { get; set; }

        public string WhatHappendsNext { get; set; }
    }
}