using System.ComponentModel;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public enum RequestStatus
    {
        [Description("New request")]
        NewRequest,

        [Description("Sent to employer")]
        SentToEmployer,

        [Description("Ready for review")]
        ReadyForReview,

        [Description("With Employer for approval")]
        WithEmployerForApproval,

        [Description("Ready for approval")]
        ReadyForApproval,

        [Description("Approved")]
        Approved
    }
}