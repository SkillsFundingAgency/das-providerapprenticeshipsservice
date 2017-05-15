using System.ComponentModel;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain
{
    public enum RecordStatus
    {
        [Description("No action needed")]
        NoActionNeeded,
        [Description("Changes pending")]
        ChangesPending,
        [Description("Changes for review")]
        ChangesForReview,
        [Description("Change requested")]
        ChangeRequested,
    }
}
