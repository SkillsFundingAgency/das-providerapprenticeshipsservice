﻿using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.CreateCohort;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public class CreateCohortMapper : ICreateCohortMapper
    {
        public CreateCohortViewModel Map(IEnumerable<RelationshipDto> source)
        {
            var result = new CreateCohortViewModel();

            var legalEntities = new List<LegalEntityViewModel>();

            foreach (var relationship in source)
            {
                legalEntities.Add(new LegalEntityViewModel
                {
                    EmployerAccountId = relationship.EmployerAccountId,
                    EmployerAccountName = relationship.EmployerAccountName,
                    EmployerAccountLegalEntityPublicHashedId = relationship.EmployerAccountLegalEntityPublicHashedId,
                    EmployerAccountLegalEntityName = relationship.EmployerAccountLegalEntityName,  
                });
            }

            result.LegalEntities = legalEntities;

            return result;
        }
    }
}