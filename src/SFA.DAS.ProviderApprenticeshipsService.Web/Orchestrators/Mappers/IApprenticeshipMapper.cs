﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public interface IApprenticeshipMapper
    {
        ApprenticeshipViewModel MapToApprenticeshipViewModel(Apprenticeship apprenticeship);

        Dictionary<string, string> MapOverlappingErrors(GetOverlappingApprenticeshipsQueryResponse overlappingErrors);

        ApprenticeshipUpdate MapFrom(ApprenticeshipUpdateViewModel viewModel);

        Apprenticeship MapFromApprenticeshipViewModel(ApprenticeshipViewModel model);

        T MapApprenticeshipUpdateViewModel<T>(Apprenticeship original, ApprenticeshipUpdate update) where T : ApprenticeshipUpdateViewModel, new();

        Task<CreateApprenticeshipUpdateViewModel> CompareAndMapToCreateUpdateApprenticeshipViewModel(Apprenticeship original, ApprenticeshipViewModel edited);

        ApprenticeshipDetailsViewModel MapFrom(Apprenticeship apprenticeship);

        Task<DataLockViewModel> MapFrom(DataLockStatus dataLock);

        TriageStatus MapTriangeStatus(SubmitStatusViewModel submitStatusViewModel);
    }
}