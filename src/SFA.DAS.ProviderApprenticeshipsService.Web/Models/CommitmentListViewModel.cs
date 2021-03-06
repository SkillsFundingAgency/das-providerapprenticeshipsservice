﻿using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class CommitmentListViewModel : ViewModelBase
    {
        public long ProviderId { get; set; }

        public IEnumerable<CommitmentListItemViewModel> Commitments { get; set; }

        public bool HasSignedAgreement { get; set; }

        // Page properties

        public string PageTitle { get; set; }

        public string PageId { get; set; }

        public string PageHeading { get; set; }

        public string PageHeading2 { get; set; }
    }
}