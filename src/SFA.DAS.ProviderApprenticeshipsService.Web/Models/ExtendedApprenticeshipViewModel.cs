using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class ExtendedApprenticeshipViewModel
    {
        public ApprenticeshipViewModel Apprenticeship { get; set; }

        public List<ITrainingProgramme> ApprenticeshipProgrammes { get; set; }

        public Dictionary<string, string> ValidationErrors { get; set; }
    }
}