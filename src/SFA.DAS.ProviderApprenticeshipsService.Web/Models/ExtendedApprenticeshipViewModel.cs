using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types.TrainingProgramme;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public sealed class ExtendedApprenticeshipViewModel
    {
        public ApprenticeshipViewModel Apprenticeship { get; set; }

        public List<TrainingProgramme> ApprenticeshipProgrammes { get; set; }

        public Dictionary<string, string> ValidationErrors { get; set; }
    }
}