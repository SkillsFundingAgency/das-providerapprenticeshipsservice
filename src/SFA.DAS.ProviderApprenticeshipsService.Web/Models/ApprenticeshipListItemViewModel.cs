using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class ApprenticeshipListItemViewModel
    {
        public long ApprenticeshipId { get; internal set; }
        public string ApprenticeshipName { get; internal set; }
        public decimal? Cost { get; internal set; }
        public DateTime? EndDate { get; internal set; }
        public DateTime? StartDate { get; internal set; }
        public string TrainingName { get; internal set; }
        public string ULN { get; internal set; }
    }
}