using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class TransferFundedViewModel : ViewModelBase
    {
        public long ProviderId { get; set; }

        public IEnumerable<TransferFundedListItemViewModel> Commitments { get; set; }

    }
}