using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Organisation;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class EmployerAccountService : IEmployerAccountService
    {
        private readonly IAccountApiClient _client;

        public EmployerAccountService(IAccountApiClient client)
        {
            _client = client;
        }

        public async Task<List<LegalEntity>> GetLegalEntitiesForAccount(string accountId)
        {
            var listOfEntities = await _client.GetLegalEntitiesConnectedToAccount(accountId);

            var list = new List<LegalEntity>();

            if (listOfEntities.Count == 0)
                return list;

            foreach (var entity in listOfEntities)
            {
                var legalEntityViewModel = await _client.GetLegalEntity(accountId, Convert.ToInt64(entity.Id));

                list.Add(new LegalEntity
                {
                    Name = legalEntityViewModel.Name,
                    RegisteredAddress = legalEntityViewModel.Address,
                    Source = legalEntityViewModel.SourceNumeric,
                    Code = legalEntityViewModel.Code,
                    Id = legalEntityViewModel.LegalEntityId,
                    AccountLegalEntityPublicHashedId = legalEntityViewModel.AccountLegalEntityPublicHashedId
                });
            }

            return list;
        }
    }
}
