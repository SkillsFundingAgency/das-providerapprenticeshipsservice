using System.Collections.Generic;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IApprenticeshipInfoServiceMapper
    {
        FrameworksView MapFrom(List<FrameworkSummary> frameworks);
        ProvidersView MapFrom(Apprenticeships.Api.Types.Providers.Provider provider);
        StandardsView MapFrom(List<StandardSummary> standards);
    }
}
