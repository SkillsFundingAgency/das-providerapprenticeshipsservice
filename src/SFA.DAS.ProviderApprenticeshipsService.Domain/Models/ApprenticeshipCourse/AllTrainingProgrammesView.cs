using System;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse
{
    public class AllTrainingProgrammesView
    {
        public DateTime CreatedDate { get; set; }
        public List<TrainingProgramme> TrainingProgrammes { get; set; }
    }
}