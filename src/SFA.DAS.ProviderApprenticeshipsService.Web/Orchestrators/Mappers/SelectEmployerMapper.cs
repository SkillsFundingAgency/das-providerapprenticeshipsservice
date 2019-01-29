using System.Collections.Generic;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderRelationships.Types.Dtos;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public class SelectEmployerMapper : ISelectEmployerMapper
    {
        public ChooseEmployerViewModel Map(IEnumerable<AccountProviderLegalEntityDto> source, EmployerSelectionAction action)
        {
            var result = new ChooseEmployerViewModel();

            var legalEntities = new List<LegalEntityViewModel>();

            foreach (var relationship in source)
            {
                legalEntities.Add(new LegalEntityViewModel
                {
                    EmployerAccountPublicHashedId = relationship.AccountPublicHashedId,
                    EmployerAccountName = relationship.AccountName,
                    EmployerAccountLegalEntityPublicHashedId = relationship.AccountLegalEntityPublicHashedId,
                    EmployerAccountLegalEntityName = relationship.AccountLegalEntityName  
                });
            }

            result.LegalEntities = legalEntities;
            result.EmployerSelectionAction = action;

            switch (action)
            {
                case EmployerSelectionAction.CreateCohort:
                    result.ControllerName = "CreateCohort";
                    result.Title = "Create Cohort";
                    result.Description = "Choose an employer you want to create a new cohort on behalf of.";
                    break;
                case EmployerSelectionAction.CreateReservation:
                    result.ControllerName = "Reservation";
                    result.Title = "Reserve Funds";
                    result.Description = "Choose an employer you want to reserve funds on behalf of.";
                    break;
            }

            return result;
        }
    }
}