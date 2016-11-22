using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    //todo: change this type to incorporate status, allowed action, etc. (don't use the API type as a view model)
    public class CommitmentListViewModel
    {
        public long ProviderId { get; set; }
        public int NumberOfTasks { get; set; }
        public List<CommitmentListItem> Commitments { get; set; }
    }
}