using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public interface IApprenticeshipCoreValidator
    {
        Dictionary<string, string> MapOverlappingErrors(GetOverlappingApprenticeshipsQueryResponse overlappingErrors);
        KeyValuePair<string, string>? CheckEndDateInFuture(DateTimeViewModel endDate);
    }
}