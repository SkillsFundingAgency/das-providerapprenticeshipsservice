using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.PAS.Account.Api.Types;

namespace SFA.DAS.PAS.Account.Api.Orchestrator
{
    public interface IAccountOrchestrator
    {
        Task<IEnumerable<User>> GetAccountUsers(long ukprn);
    }
}