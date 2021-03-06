﻿using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.DataLock.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public interface IApprenticeshipMapper
    {
        ApprenticeshipViewModel MapApprenticeship(Apprenticeship apprenticeship, CommitmentView commitment);

        Task<Apprenticeship> MapApprenticeship(ApprenticeshipViewModel vm);

        ApprenticeshipUpdate MapApprenticeshipUpdate(ApprenticeshipUpdateViewModel viewModel);

        T MapApprenticeshipUpdateViewModel<T>(Apprenticeship original, ApprenticeshipUpdate update) where T : ApprenticeshipUpdateViewModel, new();

        Task<CreateApprenticeshipUpdateViewModel> CompareAndMapToCreateUpdateApprenticeshipViewModel(Apprenticeship original, ApprenticeshipViewModel edited);

        ApprenticeshipDetailsViewModel MapApprenticeshipDetails(Apprenticeship apprenticeship);

        TriageStatus MapTriangeStatus(SubmitStatusViewModel submitStatusViewModel);
    }
}