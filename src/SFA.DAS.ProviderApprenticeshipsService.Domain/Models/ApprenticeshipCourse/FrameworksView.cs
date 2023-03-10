using System;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse
{
    public class FrameworksView
    {
        public DateTime CreatedDate { get; set; }
        public List<TrainingProgramme> Frameworks { get; set; }
    }
}