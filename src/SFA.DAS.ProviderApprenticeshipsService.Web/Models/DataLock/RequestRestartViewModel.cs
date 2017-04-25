using FluentValidation.Attributes;

using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock
{
    [Validator(typeof(RequestRestartViewModelValidator))]
    public class RequestRestartViewModel
    {
        public long ProviderId { get; set; }

        public string HashedApprenticeshipId { get; set; }

        public bool? SendRequestToEmployer { get; set; }
    }
}