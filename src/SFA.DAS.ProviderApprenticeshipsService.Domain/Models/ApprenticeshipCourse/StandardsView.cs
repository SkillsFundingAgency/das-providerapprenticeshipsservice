using System;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

public class StandardsView
{
    public DateTime CreationDate { get; set; }
    public List<TrainingProgramme> Standards { get; set; }
}