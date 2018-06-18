using System.ComponentModel;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Domain.Commitment
{
    public enum RequestStatus
    {
        None, 

        [Description("New request")]
        NewRequest,

        [Description("Under review with employer")]
        SentForReview,

        [Description("Ready for review")]
        ReadyForReview,

        [Description("With Employer for approval")]
        WithEmployerForApproval,

        [Description("Ready for approval")]
        ReadyForApproval,

        [Description("Approved")]
        Approved,

        [Description("Pending - with funding employer")]
        WithSenderForApproval,

        [Description("Rejected - with employer")]
        RejectedBySender
    }
}