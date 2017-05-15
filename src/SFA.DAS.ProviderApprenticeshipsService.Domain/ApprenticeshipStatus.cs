using System.ComponentModel;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain
{
    public enum ApprenticeshipStatus
    {
        [Description("Waiting to start")]
        WaitingToStart = 0,
        [Description("Live")]
        Live = 1,
        [Description("Paused")]
        Paused = 2,
        [Description("Stopped")]
        Stopped = 3,
        [Description("Finished")]
        Finished = 4
    }
}
