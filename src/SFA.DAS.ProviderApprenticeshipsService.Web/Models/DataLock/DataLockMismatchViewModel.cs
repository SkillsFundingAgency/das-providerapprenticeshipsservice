using FluentValidation.Attributes;
using System.Collections.Generic;

using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock
{
    [Validator(typeof(DataLockMismatchViewModelValidator))]
    public class DataLockMismatchViewModel
    {
        public List<DataLockViewModel> DataLockViewModels { get; set; }

        public ApprenticeshipViewModel DasApprenticeship { get; set; }

        public List<PriceHistoryViewModel> PriceHistory { get; set; }

        public SubmitStatusViewModel? SubmitStatusViewModel { get; set; }

        public long ProviderId { get; set; }

        public string HashedApprenticeshipId { get; set; }

        public string EmployerName { get; set; }
    }
}