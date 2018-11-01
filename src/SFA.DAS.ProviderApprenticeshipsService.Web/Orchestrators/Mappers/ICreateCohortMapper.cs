﻿using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.CreateCohort;
using SFA.DAS.ProviderRelationships.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public interface ICreateCohortMapper
    {
        CreateCohortViewModel Map(IEnumerable<ProviderRelationshipResponse.ProviderRelationship> source);
    }
}