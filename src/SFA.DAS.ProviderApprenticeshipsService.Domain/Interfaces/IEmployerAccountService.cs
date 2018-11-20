using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Organisation;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IEmployerAccountService
    {
        Task<List<LegalEntity>> GetLegalEntitiesForAccount(string accountId);
    }
}
