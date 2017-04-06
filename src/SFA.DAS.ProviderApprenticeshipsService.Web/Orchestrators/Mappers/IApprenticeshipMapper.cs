using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public interface IApprenticeshipMapper
    {
        ApprenticeshipViewModel MapToApprenticeshipViewModel(Apprenticeship apprenticeship);
        Dictionary<string, string> MapOverlappingErrors(GetOverlappingApprenticeshipsQueryResponse overlappingErrors);

        Task<UpdateApprenticeshipViewModel> CompareAndMapToUpdateApprenticeshipViewModel(Apprenticeship original, ApprenticeshipViewModel edited);
        ApprenticeshipUpdate MapFrom(UpdateApprenticeshipViewModel viewModel);
        Apprenticeship MapFromApprenticeshipViewModel(ApprenticeshipViewModel model);
    }
}
