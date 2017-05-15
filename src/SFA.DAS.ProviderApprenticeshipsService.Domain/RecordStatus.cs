using System.ComponentModel;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain
{
    public enum RecordStatus
    {
        [Description("")]
        NoActionNeeded,
        [Description("Changes Pending")]
        ChangesPending,
        [Description("Changes For Review")]
        ChangesForReview,
        [Description("Change Requested")]
        ChangeRequested,
    }
}
