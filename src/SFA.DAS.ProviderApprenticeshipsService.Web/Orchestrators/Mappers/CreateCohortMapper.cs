using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.CreateCohort;
using SFA.DAS.ProviderRelationships.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public class CreateCohortMapper : ICreateCohortMapper
    {
        public CreateCohortViewModel Map(ProviderRelationshipResponse source)
        {
            var result = new CreateCohortViewModel();

            var legalEntities = new List<LegalEntityViewModel>();

            foreach (var relationship in source.ProviderRelationships)
            {
                legalEntities.Add(new LegalEntityViewModel
                {
                    EmployerAccountId = relationship.EmployerAccountId,
                    EmployerName = relationship.EmployerName,
                    EmployerAccountLegalEntityPublicHashedId = relationship.EmployerAccountLegalEntityPublicHashedId,
                    EmployerAccountLegalEntityName = relationship.EmployerAccountLegalEntityName
                });
            }

            result.LegalEntities = legalEntities;

            return result;
        }
    }
}