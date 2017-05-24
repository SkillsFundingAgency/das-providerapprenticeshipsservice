using System.ComponentModel;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain
{
    public enum RecordStatus
    {
        [Description("No action needed")]
        NoActionNeeded = 0,
        [Description("Changes pending")]
        ChangesPending = 1,
        [Description("Changes for review")]
        ChangesForReview = 2,
        [Description("Change requested")]
        ChangeRequested = 3,
        [Description("ILR data mismatch")]
        IlrDataMismatch = 4,
        [Description("ILR changes pending")]
        IlrChangesPending = 5

    }
}
