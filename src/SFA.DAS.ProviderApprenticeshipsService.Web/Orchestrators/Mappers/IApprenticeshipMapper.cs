using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public interface IApprenticeshipMapper
    {
        ApprenticeshipViewModel MapApprenticeship(Apprenticeship apprenticeship);

        Task<Apprenticeship> MapApprenticeship(ApprenticeshipViewModel vm);

        Dictionary<string, string> MapOverlappingErrors(GetOverlappingApprenticeshipsQueryResponse overlappingErrors);

        ApprenticeshipUpdate MapApprenticeshipUpdate(ApprenticeshipUpdateViewModel viewModel);

        T MapApprenticeshipUpdateViewModel<T>(Apprenticeship original, ApprenticeshipUpdate update) where T : ApprenticeshipUpdateViewModel, new();

        Task<CreateApprenticeshipUpdateViewModel> CompareAndMapToCreateUpdateApprenticeshipViewModel(Apprenticeship original, ApprenticeshipViewModel edited);

        ApprenticeshipDetailsViewModel MapApprenticeshipDetails(Apprenticeship apprenticeship);
    }
}