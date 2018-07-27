using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes
{
    public sealed class GetTrainingProgrammesQueryResponse
    {
        public List<ITrainingProgramme> TrainingProgrammes { get; set; }
    }
}
